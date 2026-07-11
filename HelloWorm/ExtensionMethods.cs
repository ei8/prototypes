using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Spiker;
using NLog;
using System.Drawing.Drawing2D;

namespace ei8.Prototypes.HelloWorm
{
    internal static class ExtensionMethods
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Helpers
        public static string CreateUnusedName(Func<int, string> nameGenerator, Func<string, bool> nameChecker)
        {
            var result = string.Empty;

            for (int i = 1; i < int.MaxValue; i++)
            {
                string name = nameGenerator(i);
                if (!nameChecker(name))
                {
                    result = name;
                    break;
                }
            }

            return result;
        }

        // TODO: promote to ei8.Cortex.Coding.Spiker.ExtensionMethods
        public static void SubscribeReporting<T>(
            this T reporting, 
            ISpikeService spikeService, 
            Logger logger,
            EventHandler<TriggeredEventArgs> triggeredInvoker,
            EventHandler<FiredEventArgs> firedInvoker,
            Action<TriggeredEventArgs>? triggeredHandler = null, 
            Action<FiredEventArgs>? firedHandler = null
        )
            where T : ISpikableReporting2, IComponent
        {
            spikeService.Triggered += (sender, e) =>
            {
                logger.Debug(
                    new LogMessageGenerator(
                        () => {
                            var origin = e.ReflexArc.LastOrDefault();
                            var result = $"{ reporting.GetFullName()} - " +
                            $"Triggered: {e.Target.ToReadableString()}; " +
                            $"By: {(origin != null ? origin.Target.ToReadableString() : "[Stimulus]")}; " +
                            $"Count: {e.Charge.Excitations.Count() + e.Charge.Inhibitions.Count()}; " +
                            $"Charge: ({ChargeInfo.RestingPotential}) + {e.Charge.Excitations.Sum(ChargeInfo.GetCharge)} + ({e.Charge.Inhibitions.Sum(ChargeInfo.GetCharge)}) = {e.Charge.Result} mV";

                            return result;
                        }
                    )
                );

                if (triggeredHandler != null)
                    triggeredHandler(e);

                triggeredInvoker(sender, e);
            };

            spikeService.Fired += (sender, e) =>
            {
                logger.Debug(new LogMessageGenerator(() => $"{reporting.GetFullName()} - Fired: {e.FireInfo.Target.ToReadableString()}"));

                if (firedHandler != null)
                    firedHandler(e);

                firedInvoker(sender, e);
            };
        }

        // TODO: promote to ei8.Cortex.Coding.Spiker.ExtensionMethods
        public static BinaryNeuronInfo CreateBinaryNeurons(
            this Network network,
            string tagPrefix,
            string trueString = "1",
            string falseString = "0"
        ) =>
            new(
                network.CreateNeuron($"{tagPrefix} = {trueString}"),
                network.CreateNeuron($"{tagPrefix} = {falseString}")
            );

        public static InputInfo CreateTruthTableInputNeurons(
            this Network network,
            string input1TagPrefix,
            string input2TagPrefix,
            string trueString = "1",
            string falseString = "0"
        ) =>
            new(
                network.CreateBinaryNeurons(
                    input1TagPrefix,
                    trueString,
                    falseString
                ),
                network.CreateBinaryNeurons(
                    input2TagPrefix,
                    trueString,
                    falseString
                )
            );

        public static Neuron CreateNeuron(this Network network, string? tag = null)
        {
            Neuron neuron = Neuron.CreateTransient(Guid.NewGuid(), tag, null, null);
            network.AddReplace(neuron);
            return neuron;
        }
        // TODO: promote to ei8.Cortex.Coding.Spiker.ExtensionMethods
        public static Neuron CreateInterneuron(this Network network, string? interneuronTag = null, params Neuron[] postsynapticNeurons)
        {
            Neuron neuron = network.CreateNeuron(interneuronTag);

            foreach(var post in postsynapticNeurons)
                network.CreateTerminal(neuron, post);

            return neuron;
        }
        // TODO: promote to ei8.Cortex.Coding.Spiker.ExtensionMethods
        public static void LinkInputNeuronsToInterneuron(this Network network, Neuron interneuron, params Neuron[] inputNeurons)
        {
            foreach (Neuron input in inputNeurons)
                network.CreateTerminal(input, interneuron, NeurotransmitterEffect.Excite, 1f / inputNeurons.Length);
        }

        public enum LogicGateType
        {
            And,
            Or,
            Nand,
            Nor,
            Xor,
            Xnor,
            Imply,
            Nimply
        }

        public static TruthTableInterneuronInfo CreateTruthTableInterneurons(
            this Network network,
            LogicGateType type,
            BinaryNeuronInfo outputs,
            TruthTableInterneuronTagInfo? truthTableInterneuronTags = null
        )
        {
            OutputInterneuronTagInfo outputInterneuronTags = new OutputInterneuronTagInfo();

            if (truthTableInterneuronTags != null)
            {
                outputInterneuronTags = new OutputInterneuronTagInfo(
                    $"{truthTableInterneuronTags.Input1TagPrefix} = 0 {truthTableInterneuronTags.TypeTagPrefix}.{type.ToString().ToUpper()} {truthTableInterneuronTags.Input2TagPrefix} = 0",
                    $"{truthTableInterneuronTags.Input1TagPrefix} = 1 {truthTableInterneuronTags.TypeTagPrefix}.{type.ToString().ToUpper()} {truthTableInterneuronTags.Input2TagPrefix} = 0",
                    $"{truthTableInterneuronTags.Input1TagPrefix} = 0 {truthTableInterneuronTags.TypeTagPrefix}.{type.ToString().ToUpper()} {truthTableInterneuronTags.Input2TagPrefix} = 1",
                    $"{truthTableInterneuronTags.Input1TagPrefix} = 1 {truthTableInterneuronTags.TypeTagPrefix}.{type.ToString().ToUpper()} {truthTableInterneuronTags.Input2TagPrefix} = 0"
                );
            }

            switch (type)
            {
                default:
                case LogicGateType.And:
                    return network.CreateTruthTableInterneurons(
                        outputs.Neuron0, outputs.Neuron0, outputs.Neuron0, outputs.Neuron1, outputInterneuronTags
                    );
                case LogicGateType.Or:
                    return network.CreateTruthTableInterneurons(
                        outputs.Neuron0, outputs.Neuron1, outputs.Neuron1, outputs.Neuron1, outputInterneuronTags
                    );
                case LogicGateType.Nand:
                    return network.CreateTruthTableInterneurons(
                        outputs.Neuron1, outputs.Neuron1, outputs.Neuron1, outputs.Neuron0, outputInterneuronTags
                    );
                case LogicGateType.Nor:
                    return network.CreateTruthTableInterneurons(
                        outputs.Neuron1, outputs.Neuron0, outputs.Neuron0, outputs.Neuron0, outputInterneuronTags
                    );
                case LogicGateType.Xor:
                    return network.CreateTruthTableInterneurons(
                        outputs.Neuron0, outputs.Neuron1, outputs.Neuron1, outputs.Neuron0, outputInterneuronTags
                    );
                case LogicGateType.Xnor:
                    return network.CreateTruthTableInterneurons(
                        outputs.Neuron1, outputs.Neuron0, outputs.Neuron0, outputs.Neuron1, outputInterneuronTags
                    );
                case LogicGateType.Imply:
                    return network.CreateTruthTableInterneurons(
                        outputs.Neuron1, outputs.Neuron0, outputs.Neuron1, outputs.Neuron1, outputInterneuronTags
                    );
                case LogicGateType.Nimply:
                    return network.CreateTruthTableInterneurons(
                        outputs.Neuron0, outputs.Neuron1, outputs.Neuron0, outputs.Neuron0, outputInterneuronTags
                    );
            }
        }

        // TODO: remove CreateRotationInterneuron from ei8.Cortex.Coding.Spiker.ExtensionMethods
        public static TruthTableInterneuronInfo CreateTruthTableInterneurons(
            this Network network, 
            Neuron output1, 
            Neuron output2, 
            Neuron output3,
            Neuron output4,
            OutputInterneuronTagInfo outputInterneuronTags
        ) => new TruthTableInterneuronInfo(
            network.CreateInterneuron(outputInterneuronTags.Output1, output1),
            network.CreateInterneuron(outputInterneuronTags.Output2, output2),
            network.CreateInterneuron(outputInterneuronTags.Output3, output3),
            network.CreateInterneuron(outputInterneuronTags.Output4, output4)
        );

        public static void LinkTruthTableInputNeuronsToInterneurons(
            this Network network,
            TruthTableInterneuronInfo truthTableInterneurons,
            InputInfo inputs,
            params Neuron[] additionalInputNeurons
        )
        {
            network.LinkInputNeuronsToInterneuron(
                truthTableInterneurons.Interneuron1,
                [
                    inputs.Input1.Neuron0,
                    inputs.Input2.Neuron0,
                    .. additionalInputNeurons
                ]
            );
            network.LinkInputNeuronsToInterneuron(
                truthTableInterneurons.Interneuron2,
                [
                    inputs.Input1.Neuron1,
                    inputs.Input2.Neuron0,
                    .. additionalInputNeurons
                ]
            );
            network.LinkInputNeuronsToInterneuron(
                truthTableInterneurons.Interneuron3,
                [
                    inputs.Input1.Neuron0,
                    inputs.Input2.Neuron1,
                    .. additionalInputNeurons
                ]
            );
            network.LinkInputNeuronsToInterneuron(
                truthTableInterneurons.Interneuron4,
                [
                    inputs.Input1.Neuron1,
                    inputs.Input2.Neuron1,
                    .. additionalInputNeurons
                ]
            );
        }
        #endregion

        #region Forms
        public static string GetName(this ISpikableReporting2 spikable, string formDescription)
        {
            var fullName = string.Empty;
            var lifeText = string.Empty;
            if (spikable is IComponent component)
                fullName = component.GetFullName();
            if (spikable is IPerishable perishable && perishable.Life <= 0)
                lifeText = $"{(!string.IsNullOrEmpty(fullName) ? " " : string.Empty)}[Dead]";

            return 
                $"{fullName}{lifeText}" +
                $"{(!(string.IsNullOrWhiteSpace(fullName) && string.IsNullOrWhiteSpace(lifeText)) ?
                        " - " :
                        string.Empty
                    )}" +
                $"{formDescription}";
        }

        public static List<TreeNode> GetAllNodes(this TreeView treeView)
        {
            List<TreeNode> result = new List<TreeNode>();
            foreach (TreeNode child in treeView.Nodes)
            {
                result.AddRange(child.GetAllNodes());
            }
            return result;
        }

        public static List<TreeNode> GetAllNodes(this TreeNode treeNode)
        {
            List<TreeNode> result = [treeNode];
            foreach (TreeNode child in treeNode.Nodes)
            {
                result.AddRange(child.GetAllNodes());
            }
            return result;
        }
        #endregion

        #region Physical
        internal static string GetFullName(
            this IComponent component
        )
        {
            string result = component is INamed n ? 
                n.Name : 
                component.GetType().Name;

            while (component != null && component.Parent is INamed pn)
            {
                result = pn.Name + "." + result;

                if (component.Parent is IComponent cp)
                    component = cp;
            }
            return result;
        }
        internal static CollisionInfo? GetCollisionSector(
            this IRectangularComposite rectangularComposite,
            Func<SectorPointInfo, CollisionInfo?> collisionEvaluator,
            Func<float, float> angleTranslator,
            Func<Point, Point> locationTranslator
        )
        {
            CollisionInfo? result = null;
            var swps = rectangularComposite.GetSectorsPoints(
                angleTranslator,
                locationTranslator
             );

            // get sectors with the most number of collision points
            IEnumerable<CollisionInfo?> collisionsPerSector = swps
                .Select(swp => collisionEvaluator(swp));

            var collidedSectors = collisionsPerSector.Where(t =>  t != null && t.Count > 0).Select(e => e!);
            if (collidedSectors.Any())
            {
                var max = collidedSectors.DefaultIfEmpty().Max(t => t?.Count);
                if (max.HasValue && max.Value > 0)
                {
                    var maxSectors = collidedSectors.Where(ct => ct.Count == max);

                    if (maxSectors.Count() > 1)
                        ExtensionMethods.logger.Debug(
                            new LogMessageGenerator(
                                () => "Multiple sectors collided equally: " + string.Join(";", maxSectors.Select(mt => ((Sector)mt.Cause).StartAngle))
                            )
                        );

                    result = maxSectors.FirstOrDefault();
                }
            }

            return result;
        }

        internal static CollisionInfo? GetCollisionInfo(this SectorPointInfo sectorPointInfo, IEnumerable<IRectangular> components)
        {
            CollisionInfo? result = null;

            foreach (var component in components)
            {
                if (sectorPointInfo.Path.IsVisible(component.GetRectangle().Location))
                {
                    result = new(
                        component,
                        sectorPointInfo.Sector,
                        1
                    );
                    break;
                }
            }

            return result;
        }

        internal static bool IsDirectionBound(this IMovable movable, Func<float, bool> positiveRemainderEvaluator, Func<float, bool> negativeRemainderEvaluator)
        {
            bool result = false;

                float directionRemainder = Math.Abs(movable.Direction % 360f) / 360f;
                result =
                    (movable.Direction > 0 && positiveRemainderEvaluator(directionRemainder)) ||
                    (movable.Direction < 0 && negativeRemainderEvaluator(directionRemainder)); ;

            return result;
        }

        #region Sample Usage of IsDirectionBound

        /*
        // is southBound?
        bool southBound = worm.IsDirectionBound(
            // ... all positive directions between 0 and 0.5 of 360 (eg. 1 to 180 etc)
            dr => dr > 0 && dr < 0.5,
            // ... all negative directions between 0.5 and 1 of 360 (eg. -181 to -360 etc.)
            dr => dr > 0.5 && dr < 1
        );

        // is northBound?
        bool northBound = worm.IsDirectionBound(
            // ... all positive directions between 0.5 and 1 of 360 (eg. 181 to 360 etc)
            dr => dr > 0.5 && dr < 1,
            // ... all negative directions between 0.0 and 0.5 of 360 (eg. -1 to -180 etc.)
            dr => dr > 0 && dr < 0.5
        );

        var eastEvaluator = new Func<float, bool>(dr => (dr > 0 && dr < 0.25) || (dr > 0.75 && dr < 1));
        // is eastBound?
        bool eastBound = worm.IsDirectionBound(
            // ... all positive directions between 0 and 0.25 or between 0.75 and 1 of 360
            eastEvaluator,
            // ... all negative directions between 0.0 and 0.25 or between 0.75 and 1 of 360
            eastEvaluator
        );

        var westEvaluator = new Func<float, bool>(dr => dr > 0.25 && dr < 0.75);
        // is westBound?
        bool westBound = worm.IsDirectionBound(
            // ... all positive directions between 0.25 and 0.75
            westEvaluator,
            // ... all negative directions between 0.25 and 0.75
            westEvaluator
        );

        // applicable only if left/right walls 
        // ----------------------
        // or if southeastbound and sector is 1 or 2
        exclude |= southBound && eastBound && sectorId < 3;

        // or if northeastbound and sector is 7 or 8
        exclude |= northBound && eastBound && sectorId > 6;

        // or if southwestbound and sector is 7 or 8
        exclude |= southBound && westBound && sectorId > 6;

        // or if northwestbound and sector is 1 or 2
        exclude |= northBound && westBound && sectorId < 3;
        // ----------------
        */

        #endregion

        internal static void Move(this IMovable movable)
        {
            ArgumentNullException.ThrowIfNull(movable);

            Point newLocation = movable.Location.GetLocationByHypotenuseAndAngle(movable.Direction, movable.Speed);

            var mea = new MovingEventArgs(newLocation);
            movable.OnMoving(mea);

            if (mea.CollisionInfo != null)
                movable.Collide(mea.CollisionInfo);
            else
                movable.Location = newLocation;
        }

        internal static Rectangle GetRectangle(this IRectangular rectangle) => new(rectangle.Location, rectangle.Size);

        internal static IEnumerable<SectorPointInfo> GetSectorsPoints(
            this IRectangularComposite parent,
            Func<float, float> angleTranslator,
            Func<Point, Point> locationTranslator
        )
        {
            var results = new List<SectorPointInfo>();
            var sectors = parent.Components.OfType<ISector>();
            foreach (var s in sectors)
            {
                Rectangle parentRectangle = parent.GetRectangle();
                var sectorPath = new GraphicsPath();
                var sectorRectangle = new Rectangle(locationTranslator(parentRectangle.Location), parentRectangle.Size);
                sectorPath.AddPie(sectorRectangle, angleTranslator(s.StartAngle), s.SweepAngle);

                var ps = new List<Point>();
                for (int i = (int)s.StartAngle; i < s.StartAngle - 1 + s.SweepAngle; i++)
                {
                    var hp = parentRectangle.GetHypotenusePoint(angleTranslator(i));
                    hp = locationTranslator(hp);
                    ps.Add(hp);
                }

                results.Add(new(s, sectorPath, ps));
            }

            return results;
        }
        #endregion

        #region Drawing

        public static void InvalidateDish(this DishPanel dishPanel)
        {
            if (dishPanel.Dish == null)
                throw new ArgumentNullException(nameof(dishPanel.Dish));

            dishPanel.InvalidateRectangularComposite(dishPanel.Dish);
        }

        public static void InvalidateRectangularComposite(this DishPanel dishPanel, IRectangularComposite rc)
        {
            foreach (var c in rc.Components)
            {
                if (c is IRectangularComposite composite)
                {
                    dishPanel.InvalidateRectangularComposite(composite);
                }
                else if (c is IRectangular r)
                {
                    dishPanel.InvalidateRectangle(r, Constants.Render.RegularOffset);
                }
            }

            if (rc is not Dish)
            {
                var offset = rc is Worm w ? w.Size.Height : Constants.Render.RegularOffset;
                dishPanel.InvalidateRectangle(rc, offset);
            }
        }

        private static void InvalidateRectangle(this DishPanel dishPanel, IRectangular r, int offset)
        {
            var cr = r.GetRectangle();
            cr = cr.Resize(offset);
            dishPanel.Invalidate(cr, false);
        }

        private static Rectangle Resize(this Rectangle cr, int offset)
        {
            cr.Offset(-offset, -offset);
            cr.Size = new System.Drawing.Size(cr.Size.Width + (offset * 2), cr.Size.Height + (offset * 2));
            return cr;
        }

        static Brush GetRandomBrushColor(int maxValue)
        {
            var random = new Random();
            var rv = random.Next(maxValue);
            var rb = Brushes.Green;
            switch (rv)
            {
                case 1:
                    rb = Brushes.Red;
                    break;
                case 2:
                    rb = Brushes.Blue;
                    break;
            }

            return rb;
        }

        internal static Point GetLocationByHypotenuseAndAngle(this Point original, float angle, double hypotenuse)
        {
            // 0 degrees is East
            var rad = angle * (Math.PI / 180d);

            var newLocation = new Point(
                original.X + Convert.ToInt32(Math.Cos(rad) * hypotenuse),
                original.Y + Convert.ToInt32(Math.Sin(rad) * hypotenuse)
            );

            return newLocation;
        }

        internal static Point GetHypotenusePoint(this Rectangle rectangle, float angle, int offset = 0)
        {
            var halfOfRectangle = rectangle.Size / 2;
            var centerOfRectangle = rectangle.Location.Add(new Point(halfOfRectangle.Width, halfOfRectangle.Height));
            Point hypoPoint = centerOfRectangle.GetLocationByHypotenuseAndAngle(
                angle,
                halfOfRectangle.Width + offset
            );
            return hypoPoint;
        }

        internal static void DrawCenteredStringAtPoint(this Graphics g, string text, Font font, Brush brush, PointF centerPoint)
        {
            // 1. Define a rectangle around the center point. 
            //    The size needs to be large enough to contain the string.
            //    A common approach is to use MeasureString to estimate the required size.
            SizeF stringSize = g.MeasureString(text, font);

            // Calculate the top-left corner of the rectangle to center it on the centerPoint
            float rectX = centerPoint.X - (stringSize.Width / 2);
            float rectY = centerPoint.Y - (stringSize.Height / 2);
            RectangleF layoutRect = new RectangleF(rectX, rectY, stringSize.Width, stringSize.Height);

            // 2. Create a StringFormat object and set the alignment properties
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;      // Horizontal center
            sf.LineAlignment = StringAlignment.Center;  // Vertical center

            // 3. Draw the string within the defined rectangle using the StringFormat
            g.DrawString(text, font, brush, layoutRect, sf);
        }

        internal static void DrawGrid(this Graphics g, System.Drawing.Size dishSize, int gap)
        {
            using Pen gridPen = new Pen(Color.Gray);
            for (int i = gap; i <= dishSize.Width; i += gap)
                g.DrawLine(gridPen, new Point(i, 0), new Point(i, dishSize.Height));

            for (int i = gap; i <= dishSize.Height; i += gap)
                g.DrawLine(gridPen, new Point(0, i), new Point(dishSize.Width, i));
        }

        internal static void DrawRectangleBound(
            this Graphics g, 
            IRectangleBound rectangleBound, 
            IRectangularComposite parent, 
            Func<float, float> angleTranslator,
            bool showSectorIds,
            params Func<Point, Point>[] locationTranslators
        )
        {
            if (rectangleBound is ISector sector)
            {
                using var boundedPath = new GraphicsPath();
                var parentRectangle = parent.GetRectangle();
                parentRectangle.Location = parentRectangle.Location.Translate(locationTranslators);

                Brush rb = ExtensionMethods.GetRandomBrushColor(2);
                g.FillPie(rb, parentRectangle, angleTranslator(sector.StartAngle), sector.SweepAngle);

                if (showSectorIds)
                {
                    Point hypoPoint = parentRectangle.GetHypotenusePoint(
                        angleTranslator(sector.StartAngle - 1 + (sector.SweepAngle / 2)),
                        10
                    );
                    g.DrawCenteredStringAtPoint(
                        sector.Name.Substring(typeof(Sector).Name.Length),
                        new Font("Arial", 8, FontStyle.Regular),
                        Brushes.Blue,
                        hypoPoint
                    );
                }
            }
        }

        internal static void DrawRectangular(
            this Graphics g, 
            IRectangular rectangular, 
            Pen pen,
            Func<float, float> angleTranslator,
            bool showSectorIds,
            params Func<Point, Point>[] locationTranslators
        )
        {
            g.DrawRectangle(
                rectangular,
                pen,
                (irect) => new(
                    irect.Location.Translate(locationTranslators),
                    irect.Size
                )
            );

            if (rectangular is IRectangularComposite rectangularComposite)
                g.DrawRectangularComposite(rectangularComposite, pen, angleTranslator, showSectorIds, locationTranslators);
        }

        internal static void DrawRectangularComposite(
            this Graphics g, 
            IRectangularComposite rectangularComposite, 
            Pen pen, 
            Func<float, float> angleTranslator,
            bool showSectorIds,
            params Func<Point, Point>[] locationTranslators
        )
        {
            foreach (var component in rectangularComposite.Components)
            {
                if (component is IRectangular rectangularComponent)
                {
                    g.DrawRectangular(
                        rectangularComponent,
                        pen,
                        angleTranslator,
                        showSectorIds,
                        (pt) => pt.Translate(
                            locationTranslators.Prepend((point) => point.Add(rectangularComposite.Location))
                        )
                    );
                }
                else if (component is IRectangleBound rectangleBound)
                {
                    g.DrawRectangleBound(
                        rectangleBound,
                        rectangularComposite, 
                        angleTranslator,
                        showSectorIds,
                        locationTranslators
                    );
                }
            }
        }

        internal static void DrawRectangle(
            this Graphics g,
            IRectangular rectangular,
            Pen pen
        )
            => DrawRectangle(g, rectangular, pen, irect => irect.GetRectangle());

        internal static void DrawRectangle(
            this Graphics g,
            IRectangular rectangular,
            Pen pen,
            Func<IRectangular, Rectangle> rectangleRetriever
        )
        {
            var rect = rectangleRetriever(rectangular);
            if (rectangular is IElliptical elliptical)
                g.DrawEllipse(pen, rect);
            else
                g.DrawRectangle(pen, rect);
        }

        internal static Point Translate(
            this Point original,
            IEnumerable<Func<Point, Point>> translators
        ) => translators.Any() ? translators.Select(t => original = t(original)).Last() : original;

        internal static Point Add(this Point firstPoint, Point secondPoint) =>
            new Point(firstPoint.X + secondPoint.X, firstPoint.Y + secondPoint.Y);
        #endregion
    }
}

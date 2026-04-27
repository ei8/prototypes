using ei8.Cortex.Coding;
using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Reflection;
using ei8.Prototypes.HelloWorm.Spiker;
using neurUL.Common.Domain.Model;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using System.Reflection;
using Neuron = ei8.Cortex.Coding.Neuron;
using Terminal = ei8.Cortex.Coding.Terminal;

namespace ei8.Prototypes.HelloWorm
{
    internal static class ExtensionMethods
    {
        #region Spiker
        /// <summary>
        /// As indicated, this is a temporary approach. 
        /// Ideally, the fired neurons for a method and its parameters
        /// should be retrieved via mirrors if necessary, deneurULized, cached and invoked accordingly. 
        /// eg. Rotate Method (granny), Clockwise Direction Parameter (granny), 22.5 Float Degrees Parameter (granny)
        /// Using Method (class), MethodParameter (class)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="fireInfos"></param>
        /// <param name="methodNeuronId"></param>
        /// <param name="param1ValueMap"></param>
        /// <param name="param2ValueMap"></param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <returns></returns>
        internal static bool TryFauxDeneurULizeInvoke<T1, T2>(
            this IEnumerable<FireInfo> fireInfos,
            Guid methodNeuronId,
            IDictionary<Guid, T1> param1ValueMap,
            IDictionary<Guid, T2> param2ValueMap,
            [NotNullWhen(true)] out T1? parameter1,
            [NotNullWhen(true)] out T2? parameter2
        )
            where T1 : struct
            where T2 : struct
        {
            bool result = false;
            parameter1 = default;
            parameter2 = default;

            FireInfo? latestFire = fireInfos.LastOrDefault();
            // if last fired is one of the anticipated neurons
            // TODO: anticipated neurons can include instantiates grannies (eg. instantiates^methodParameter) to optimize recognition,
            // ie. no need to recognize all possible values
            if (
                latestFire.HasValue &&
                (
                    latestFire.Value.Target.Id == methodNeuronId ||
                    param1ValueMap.ContainsKey(latestFire.Value.Target.Id) ||
                    param2ValueMap.ContainsKey(latestFire.Value.Target.Id)
                )
            )
            {
                // if number of related fires equals method + 2 parameters
                if (fireInfos.Count() >= 3)
                {
#if DEBUG
                    //Debug.WriteLine($"Related Fires (Micros): {string.Join(
                    //        ", ",
                    //        neuronFireInfos.Select(rf =>
                    //            rf.Neuron.Tag +
                    //            " (" +
                    //            latestFire.FireInfo.Timestamp.Subtract(rf.FireInfo.Timestamp).TotalMicroseconds +
                    //            ")"
                    //         )
                    //    )}");
#endif
                    if (
                        // and specified method was fired
                        fireInfos.Any(n => n.Target.Id == methodNeuronId) &&
                        // and any param1 was fired
                        fireInfos.TryGetFiredParameter(param1ValueMap, out parameter1) &&
                        // and any param2 was fired
                        fireInfos.TryGetFiredParameter(param2ValueMap, out parameter2)
                    )
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        private static bool TryGetFiredParameter<T1>(this IEnumerable<FireInfo> fireInfos, IDictionary<Guid, T1> paramValueMap, out T1? parameter) where T1 : struct
        {
            parameter = null;

            foreach (var nfi in fireInfos)
                if (paramValueMap.ContainsKey(nfi.Target.Id))
                {
                    parameter = paramValueMap[nfi.Target.Id];
                    break;
                }

            return parameter != null;
        }

        public static void Clean<T>(this ConcurrentDictionary<DateTime, T> concurrentDictionary, Func<T, DateTime> timestampRetriever, DateTime maxTimestamp)
        {
            foreach (var nfi in concurrentDictionary.Values)
            {
                var ts = timestampRetriever(nfi);
                if (ts < maxTimestamp)
                    concurrentDictionary.Remove(ts, out _);
            }
        }

        public static Neuron ValidateGet(this Network network, Guid id)
        {
            if (network.TryGetById(id, out Neuron neuron))
                return neuron;
            else
                throw new ArgumentException($"Neuron with specified Id '{id}' was not found.");
        }

        public static string ToString(this Neuron neuron)
        {
            return $"{neuron.Id}:Neuron '{neuron.Tag}'";
        }

        public static bool TryGetAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueCreator, out TValue? result)
            where TKey : notnull
        {
            var bResult = false;

            if (!dictionary.ContainsKey(key))
                dictionary.TryAdd(key, valueCreator(key));

            if (dictionary.TryGetValue(key, out TValue? getResult))
            {
                bResult = true;
                result = getResult;
            }
            else
                result = default;

            return bResult;
        }

        public static Neuron CreateInputNeuron(this Network network, MirrorConfig mirrorConfig, float strengthToInterneurons, params Neuron[] interneurons)
        {
            AssertionConcern.AssertArgumentNotNull(mirrorConfig, nameof(mirrorConfig)); 

            var result = network.CreateNeuron(mirrorConfig);

            foreach (var interneuron in interneurons)
                network.CreateTerminal(result, interneuron, NeurotransmitterEffect.Excite, strengthToInterneurons);

            return result;
        }

        public static Neuron CreateRotationInterneuron(this Network network, Neuron rotateNeuron, Neuron directionNeuron, Neuron degreesNeuron)
        {
            var result = network.CreateNeuron();

            network.CreateTerminal(result, rotateNeuron);
            network.CreateTerminal(result, directionNeuron);
            network.CreateTerminal(result, degreesNeuron);

            return result;
        }

        public static Neuron CreateNeuron(
            this Network network,
            MirrorConfig mirrorConfig
        )
        {
            AssertionConcern.AssertArgumentNotNull(mirrorConfig, nameof(mirrorConfig));

            var result = Neuron.CreateTransient(Guid.NewGuid(), string.Join(',', mirrorConfig.Keys), mirrorConfig.Url, null);
            network.AddReplace(result);
            return result;
        }

        public static Neuron CreateNeuron(
            this Network network
        )
        {
            var result = Neuron.CreateTransient(Guid.NewGuid(), null, null, null);
            network.AddReplace(result);
            return result;
        }

        public static Terminal CreateTerminal(
            this Network network,
            Neuron presynapticNeuron,
            Neuron postsynapticNeuron
        ) => network.CreateTerminal(presynapticNeuron, postsynapticNeuron, NeurotransmitterEffect.Excite, 1f);

        public static Terminal CreateTerminal(
            this Network network,
            Neuron presynapticNeuron,
            Neuron postsynapticNeuron,
            NeurotransmitterEffect effect,
            float strength
        )
        {
            var result = new Terminal(Guid.NewGuid(), presynapticNeuron.Id, postsynapticNeuron.Id, effect, strength);
            network.AddReplace(result);
            return result;
        }

        public static IDictionary<Guid, T> ConvertToNeuronValueMap<T>(this IEnumerable<T> values, IEnumerable<MirrorConfig> mirrorConfigs, Network network) where T : Enum
        {
            var result = new Dictionary<Guid, T>();

            foreach (var value in values) 
            {
                Neuron? neuron = null;
                if (
                    !mirrorConfigs.TryGetMirrorNeuron(
                        value.ToEnumKeyString(),
                        network,
                        out neuron
                    )
                )
                    throw new InvalidOperationException($"Failed retrieving NeuronValueMap for {value.ToEnumKeyString()}");

                result.Add(neuron.Id, value);
            }

            return result;
        }

        #region TODO: Promote to ei8.Cortex.Coding
        public static bool TryGetMirrorNeuron(this IEnumerable<MirrorConfig> mirrorConfigs, string key, Network network, [NotNullWhen(true)] out Neuron? result)
        {
            result = null;

            return mirrorConfigs.TryGetByKey(
                key,
                out MirrorConfig? config
            ) &&
            network.TryGetByMirrorUrl(
                config.Url,
                out result
            );
        }

        public static bool TryGetByMirrorUrl(this Network network, string mirrorUrl, [NotNullWhen(true)] out Neuron? result)
        {
            bool bResult = false;
            result = null;
            var neurons = from n in network.GetItems<Neuron>()
                where n.MirrorUrl == mirrorUrl
                select n;

            if (neurons.Any())
            {
                result = neurons.Single();
                bResult = true;
            }

            return bResult;
        }

        public static bool TryGetByKey(this IEnumerable<MirrorConfig> configs, string key, [NotNullWhen(true)] out MirrorConfig? result)
        {
            result = configs.SingleOrDefault(c => c.Keys.Any(ck => ck == key));
            return result != null;
        }

        public static string ToEnumKeyString(this Enum value)
        {
            return $"{value.GetType().ToKeyString()}|{value.ToString()}";
        }

        public static string ToMethodKeyString(this Type type, string methodName, params Type[] parameterTypes)
        {
            return type.GetMethod(
                methodName, 
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, 
                null, 
                parameterTypes, 
                null
            )!.ToMethodKeyString();
        }

        public static string ToMethodKeyString(this MethodInfo value)
        {
            if (value is MethodInfo methodInfo)
            {
                return methodInfo.ReflectedType.ToKeyString() + ";" + methodInfo.Name;
            }

            throw new ArgumentOutOfRangeException(nameof(value));
        }
        #endregion
        #endregion

        #region Physical
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

#if DEBUG
                    if (maxSectors.Count() > 1)
                        Debug.WriteLine("Multiple sectors collided equally: " + string.Join(";", maxSectors.Select(mt => ((Sector)mt.Source).StartAngle)));
#endif

                    result = maxSectors.FirstOrDefault();
                }
            }

            return result;
        }

        internal static CollisionInfo? GetCollisionInfo(this SectorPointInfo sp, IEnumerable<IRectangular> components)
        {
            CollisionInfo? result = null;

            foreach (var c in components)
            {
                if (sp.Path.IsVisible(c.GetRectangle().Location))
                {
                    result = new(
                        c,
                        sp.Sector,
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

        internal static int GetSectorId(this IRectangularComposite rectangularComposite, ISectoral sectoral) =>
            rectangularComposite.Components.TakeWhile(s => s != sectoral).Count() + 1;

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
            var sectors = parent.Components.OfType<IRectangleBoundSectoral>();
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
                var offset = rc is Worm w ? (w.Size.Width * 2) : Constants.Render.RegularOffset;
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
            cr.Size = new Size(cr.Size.Width + (offset * 2), cr.Size.Height + (offset * 2));
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

        internal static void DrawGrid(this Graphics g, Size dishSize, int gap)
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
            if (rectangleBound is ISectoral sectoral)
            {
                using var boundedPath = new GraphicsPath();
                var parentRectangle = parent.GetRectangle();
                parentRectangle.Location = parentRectangle.Location.Translate(locationTranslators);

                Brush rb = ExtensionMethods.GetRandomBrushColor(2);
                g.FillPie(rb, parentRectangle, angleTranslator(sectoral.StartAngle), sectoral.SweepAngle);

                if (showSectorIds)
                {
                    Point hypoPoint = parentRectangle.GetHypotenusePoint(
                        angleTranslator(sectoral.StartAngle - 1 + (sectoral.SweepAngle / 2)),
                        10
                    );
                    g.DrawCenteredStringAtPoint(
                        parent.GetSectorId(sectoral).ToString(),
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

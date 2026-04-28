
namespace ei8.Prototypes.HelloWorm
{
    public class ProjectService : IProjectService
    {
        private Project? project;

        public event EventHandler? ProjectChanged;
        public event EventHandler? ProjectChanging;

        public Project? GetProject() => this.project;

        public void SetProject(Project project)
        {
            if (this.project !=  project)
            {
                this.ProjectChanging?.Invoke(this, EventArgs.Empty);

                this.project = project;

                this.ProjectChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

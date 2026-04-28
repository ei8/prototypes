namespace ei8.Prototypes.HelloWorm
{
    public interface IProjectService
    {
        event EventHandler ProjectChanged;
     
        event EventHandler ProjectChanging;

        void SetProject(Project project);

        Project? GetProject();
    }
}

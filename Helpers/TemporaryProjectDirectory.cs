using System.IO;

namespace AppComposer.Helpers
{

    public sealed class TemporaryProjectDirectory : IDisposable
    {
        public string Path { get; }

        public TemporaryProjectDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "AppComposer", Guid.NewGuid().ToString());

            Directory.CreateDirectory(Path);
        }

        public void Dispose()
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
    }
}

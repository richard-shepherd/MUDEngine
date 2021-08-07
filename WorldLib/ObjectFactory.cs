using System.IO;

namespace WorldLib
{
    /// <summary>
    /// Creates game objects, ie objects derived from ObjectBase.
    /// </summary>
    public class ObjectFactory
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public ObjectFactory()
        {
        }

        /// <summary>
        /// Adds a root folder from which object definitions will be loaded.
        /// All .json files in this folder and sub-folders will be loaded and
        /// can be used to create objects.
        /// </summary>
        public void addRootFolder(string path)
        {
            // We find all .json files in this folder and sub-folders, and load them...
            path = Path.GetFullPath(path);
            var objectfilePaths = Directory.EnumerateFiles(path, "*.json", SearchOption.AllDirectories);
            foreach(var objectFilePath in objectfilePaths)
            {

            }
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Loads JSON config from the path specified.
        /// </summary>
        private void loadObjectFile(string path)
        {
            try
            {

            }
            catch(Exception ex)
            {
            }
        }

        #endregion
    }
}

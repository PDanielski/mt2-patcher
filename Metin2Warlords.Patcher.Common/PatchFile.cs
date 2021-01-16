namespace Metin2Warlords.Patcher.Common
{
    public class PatchFile
    {
        public string Name { get; internal set; }
        public string MD5 { get; internal set; }
        public string BasePath { get; internal set; }
        public int Size { get; internal set; }

        public PatchFile(string name, string MD5, string basePath, int size)
        {
            this.Name = name;
            this.MD5 = MD5;
            this.BasePath = basePath;
            this.Size = size;
        }

    }
}

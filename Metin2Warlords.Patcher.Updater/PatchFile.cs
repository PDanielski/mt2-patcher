using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdatePatcher
{
    class PatchFile
    {
        private String name;
        private String MD5;
        private int size;

        public PatchFile(String name, String MD5, int size)
        {
            this.name = name;
            this.MD5 = MD5;
            this.size = size;
        }

        public String getName()
        {
            return name;
        }

        public String getMD5()
        {
            return MD5;
        }

        public int getSize()
        {
            return size;
        }
    }
}

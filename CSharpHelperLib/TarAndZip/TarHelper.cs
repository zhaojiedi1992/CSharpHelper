using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;

namespace CSharpHelperLib.TarAndZip
{


    public enum CompressesType
    {
        BZip,
        GZip,
        Iso,
        Zip,
        Cab,
        Arj,
        Lzh,
        Ace,
        Zip7,
        Uue,
        Bz2,
        Jar,
        Z,
        Rar,
        Tar,

       Unknown

    }
  public  class TarHelper
    {

        public void ExtractTar(String filename, String dstFolder)
        {
          
            CompressesType compressesType = GuessCompressesType(filename);
          
            if (compressesType.ToString().ToLower().Contains("tar"))
            {
                Stream inStream = File.OpenRead(filename);
                Stream compresseStream = Stream.Null;
                switch (compressesType)
                {
                    case CompressesType.BZip:
                        compresseStream = new BZip2InputStream(inStream);
                        break;
                    case CompressesType.GZip:
                        compresseStream = new GZipInputStream(inStream);
                        break;
                    case CompressesType.Iso:
                        //compresseStream=new IsolatedStorageFileStream(inStream);
                        break;
                    case CompressesType.Zip:
                        compresseStream = new ZipInputStream(inStream);
                        break;
                    case CompressesType.Unknown:
                        break; throw new Exception("compressType is unknow");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
               
                TarArchive tarArchive = TarArchive.CreateInputTarArchive(compresseStream);
                tarArchive.ExtractContents(dstFolder);
                tarArchive.Close();
                inStream.Close();
            }
            else
            {
                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(filename,dstFolder,null);
            }
          
        }

        private CompressesType GuessCompressesType(string filename)
        {
            filename = filename.ToLower();
            foreach (CompressesType bt in (CompressesType[])Enum.GetValues(typeof(CompressesType)))
            {
                if (Path.GetExtension(filename).ToLower().TrimStart('.') == bt.ToString().ToLower())
                {
                    return bt;
                }
            }
            return CompressesType.Unknown;
        }
    }
}

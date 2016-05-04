using System;
using System.IO;
using System.Linq;

namespace AddressProcessing.CSV
{
    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

    public class CSVReaderWriter
    {
        private StreamReader _readerStream = null;
        private StreamWriter _writerStream = null;
        private const string TabSpace = "\t"; //have a constants class to hold all constant in one central place

        //[Flags] this will when using multi value and not with bitwise enum operator.
        public enum Mode { Read = 1, Write = 2 };

        public void Open(string filePath, Mode mode)
        {
            if (!Enum.IsDefined(typeof (Mode), mode))
            {
                throw new Exception("Unknown file mode for " + filePath);
            }

            FileInfo fileInfo = new FileInfo(filePath);


            if (mode == Mode.Read) //the right way is to use hasFlags rather than == if it is multi valued. Assuming this is already in production, the code read/write works mutually exclusive to each other.
            {
                if (!fileInfo.Exists)
                {
                    throw new Exception("File Missing: " + filePath);
                }
                _readerStream = fileInfo.OpenText();
            }
            if (mode == Mode.Write)
            {
                _writerStream = fileInfo.CreateText();
            }
        }

        public void Write(params string[] columns)
        {
            if (_writerStream == null || //dont like it but have to use it this way due to backward compatibility with existing code. Ideally would prefer read and write operation stand alone.
                columns == null ||
                columns.Length == 0)
            {
                throw new Exception("No Stream found. Open the stream before writing.");
            }

            var columnsString = string.Join(TabSpace, columns);
            _writerStream.WriteLine(columnsString);
        }

        public bool Read(out string column1, out string column2)
        {
            column1 = null;
            column2 = null;

            if (_readerStream == null)
            {
                throw new Exception("No Stream found. Open the stream before reading.");
            }

            var columnsString = _readerStream.ReadLine();

            if (string.IsNullOrEmpty(columnsString))
            {
                return false;
            }

            var columns = columnsString.Split(new []{ TabSpace }, StringSplitOptions.None);

            if (columns.Length == 0)
            {
                return false;
            }

            column1 = columns[0];
            column2 = columns[1];

            return true;
        }

        public void Close()
        {
            if (_writerStream != null)
            {
                _writerStream.Close();
            }

            if (_readerStream != null)
            {
                _readerStream.Close();
            }
        }
    }
}

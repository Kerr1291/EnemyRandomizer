using System.Xml.Serialization;
using System.IO;

namespace nv
{
    public class XMLUtils
    {
        /*
        [XmlRoot("AppSettings")]
        public class ExampleData
        {
            [XmlElement("Data")]
            public string data;
            [XmlElement("MoreData")]
            public string moreData;

            [XmlArray("ListOfData")]
            public List<string> someListOfData;

            [XmlElement(ElementName ="OptionalData", IsNullable = true)]
            public bool? someOptionalData;
        }
        */

        public static bool WriteDataToFile<T>(string path, T data) where T : class
        {
            bool result = false;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            FileStream fstream = null;
            try
            {
                fstream = new FileStream(path, FileMode.Create);
                serializer.Serialize(fstream, data);
                result = true;
            }
            catch(System.Exception e)
            {
                Dev.LogError( e.Message );
                //System.Windows.Forms.MessageBox.Show("Error creating/saving file "+ e.Message);
            }
            finally
            {
                fstream.Close();
            }
            return result;
        }

        public static bool ReadDataFromFile<T>(string path, out T data) where T : class
        {
            data = null;

            if(!File.Exists(path))
            {
                //System.Windows.Forms.MessageBox.Show("No file found at " + path );
                return false;
            }

            bool returnResult = true;

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            FileStream fstream = null;
            try
            {
                fstream = new FileStream(path, FileMode.Open);
                data = serializer.Deserialize(fstream) as T;
            }
            catch(System.Exception e)
            {
                Dev.LogError( e.Message );
                //System.Windows.Forms.MessageBox.Show("Error loading file " + e.Message);
                returnResult = false;
            }
            finally
            {
                fstream.Close();
            }

            return returnResult;
        }
    }
}
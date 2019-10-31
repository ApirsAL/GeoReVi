using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GeoReVi
{
    public class AbstractSerializer<T> : IXmlSerializable
    {
        #region Properties
        public T AbstractObject
        {
            get { return abstractObject; }
        }
        T abstractObject;
        #endregion Properties

        #region Constructors
        public AbstractSerializer() { }

        public AbstractSerializer(T abstractObject)
        {
            this.abstractObject = abstractObject;
        }
        #endregion Constructors

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Type type = Type.GetType(reader.GetAttribute("type"));
            reader.ReadStartElement();
            this.abstractObject = (T)new
                          XmlSerializer(type).Deserialize(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("type", abstractObject.GetType().ToString());
            new XmlSerializer(abstractObject.GetType()).Serialize(writer, abstractObject);
        }
    }
}

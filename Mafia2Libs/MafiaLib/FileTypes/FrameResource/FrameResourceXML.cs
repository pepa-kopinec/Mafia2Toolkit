﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace ResourceTypes.FrameResource
{
    public class BaseResource
    {
        protected readonly int SerialVersion;
        protected readonly string TypeName;
        protected int EntryVersion;
        protected string FileName;

        public BaseResource(int version, string name)
        {
            TypeName = name;
            SerialVersion = version;
        }

        public BaseResource()
        {
        }

        public virtual int GetEntryVersion()
        {
            return EntryVersion;
        }

        public virtual string GetEntryVersionString()
        {
            return EntryVersion.ToString();
        }

        public virtual void SetEntryVersion(int version)
        {
            EntryVersion = version;
        }

        public virtual int GetSerializationVersion()
        {
            return SerialVersion;
        }

        public virtual string GetSerializationVersionString()
        {
            return SerialVersion.ToString();
        }

        public virtual string GetTypeName()
        {
            return TypeName;
        }

        public virtual string GetFileName()
        {
            return FileName;
        }

        public virtual void SetFileName(string file)
        {
            FileName = file;
        }

        public virtual void ReadResourceEntry(XPathNodeIterator iterator)
        {
            iterator.Current.MoveToNext();
            FileName = iterator.Current.Value;
            iterator.Current.MoveToNext();
            EntryVersion = iterator.Current.ValueAsInt;
        }

        public virtual void WriteResourceEntry(XmlWriter writer)
        {
            writer.WriteElementString("File", GetFileName());
            writer.WriteElementString("Version", GetEntryVersionString());
        }
    }
    public class TableResource : BaseResource
    {
        private string[] tables;

        public string[] Tables {
            get { return tables; }
            set { tables = value; }
        }
        public TableResource()
        {

        }

        public TableResource(int version, string name) : base(version, name)
        {
        }

        public override void ReadResourceEntry(XPathNodeIterator iterator)
        {
            iterator.Current.MoveToNext();
            int numTables = iterator.Current.ValueAsInt;
            Tables = new string[numTables];
            for (int i = 0; i < numTables; i++)
            {
                iterator.MoveNext();
                Tables[i] = iterator.Current.Value;
            }
            iterator.Current.MoveToNext();
            EntryVersion = iterator.Current.ValueAsInt;
        }

        public override void WriteResourceEntry(XmlWriter writer)
        {
            writer.WriteElementString("NumTables", Tables.Length.ToString());
            for(int i = 0; i < Tables.Length; i++)
            {
                writer.WriteElementString("Table", Tables[i]);
            }
            writer.WriteElementString("Version", GetEntryVersionString());
        }
    }
    public class ScriptResource : BaseResource
    {
        private string[] scripts = null;

        public string[] Scripts {
            get { return scripts; }
            set { scripts = value; }
        }

        public ScriptResource()
        {

        }

        public ScriptResource(int version, string name) : base(version, name)
        {

        }

        public override void ReadResourceEntry(XPathNodeIterator iterator)
        {
            iterator.Current.MoveToNext();
            FileName = iterator.Current.Value;
            iterator.Current.MoveToNext();
            int numScripts = iterator.Current.ValueAsInt;
            Scripts = new string[numScripts];
            for(int i = 0; i < numScripts; i++)
            {
                iterator.Current.MoveToNext();
                Scripts[i] = iterator.Current.Value;
            }
            iterator.Current.MoveToNext();
            EntryVersion = iterator.Current.ValueAsInt;
        }

        public override void WriteResourceEntry(XmlWriter writer)
        {
            writer.WriteElementString("File", GetFileName());
            writer.WriteElementString("ScriptNum", Scripts.Length.ToString());
            for(int i = 0; i < Scripts.Length; i++)
            {
                writer.WriteElementString("Name", Scripts[i]);
            }
            writer.WriteElementString("Version", GetEntryVersionString());
        }
    }
    public class XMLResource : BaseResource
    {
        private string xmlTag;
        private int unk1;
        private int unk3;

        public string XMLTag {
            get { return xmlTag; }
            set { xmlTag = value; }
        }
        public int Unk1 {
            get { return unk1; }
            set { unk1 = value; }
        }
        public int Unk3 {
            get { return unk3; }
            set { unk3 = value; }
        }

        public XMLResource()
        {

        }

        public XMLResource(int version, string name) : base(version, name)
        {

        }

        public override void ReadResourceEntry(XPathNodeIterator iterator)
        {
            base.ReadResourceEntry(iterator);
            iterator.Current.MoveToNext();
            XMLTag = iterator.Current.Value;
            iterator.Current.MoveToNext();
            Unk1 = iterator.Current.ValueAsInt;
            iterator.Current.MoveToNext();
            Unk3 = iterator.Current.ValueAsInt;
        }

        public override void WriteResourceEntry(XmlWriter writer)
        {
            writer.WriteElementString("File", GetFileName());
            writer.WriteElementString("XMLTag", XMLTag);
            writer.WriteElementString("Unk1", Unk1.ToString());
            writer.WriteElementString("Unk3", Unk3.ToString());
            writer.WriteElementString("Version", GetEntryVersionString());
        }
    }
    public class TextureResource : BaseResource
    {
        private int hasMIP;

        public int HasMIP {
            get { return hasMIP; }
            set { hasMIP = value; }
        }

        public TextureResource()
        {

        }

        public TextureResource(int version, string name) : base(version, name)
        {

        }

        public override void ReadResourceEntry(XPathNodeIterator iterator)
        {
            iterator.Current.MoveToNext();
            FileName = iterator.Current.Value;
            iterator.Current.MoveToNext();
            HasMIP = iterator.Current.ValueAsInt;
            iterator.Current.MoveToNext();
            EntryVersion = iterator.Current.ValueAsInt;
        }

        public override void WriteResourceEntry(XmlWriter writer)
        {
            writer.WriteElementString("File", GetFileName());
            writer.WriteElementString("HasMIP", HasMIP.ToString());
            writer.WriteElementString("Version", GetEntryVersionString());
        }
    }
}

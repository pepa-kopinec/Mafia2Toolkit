﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mafia2
{
    public class VertexBufferManager
    {
        VertexBufferPool[] bufferPools;
        List<FileInfo> loadedPoolNames;

        public VertexBufferPool[] BufferPools {
            get { return bufferPools; }
            set { bufferPools = value; }
        }
        public List<FileInfo> LoadedPoolNames {
            get { return loadedPoolNames; }
            set { loadedPoolNames = value; }
        }

        /// <summary>
        /// Construct manager with passed files.
        /// </summary>
        /// <param name="files"></param>
        public VertexBufferManager(List<FileInfo> files)
        {
            loadedPoolNames = files;
            ReadFiles();
        }

        /// <summary>
        /// Get buffer from manager. use IndexBufferRef from FrameGeometry.
        /// </summary>
        /// <param name="indexRef">indexBufferRef</param>
        /// <returns></returns>
        public BufferLocationStruct SearchBuffer(ulong indexRef)
        {
            for (int i = 0; i != bufferPools.Length; i++)
            {
                int c = 0;
                foreach (KeyValuePair<ulong, VertexBuffer> entry in bufferPools[i].Buffers)
                {
                    if (entry.Key == indexRef)
                        return new BufferLocationStruct(i, c);
                    c++;
                }
            }
            return null;
        }


        /// <summary>
        /// Read files which are passed through constructor.
        /// </summary>
        public void ReadFiles()
        {
            bufferPools = new VertexBufferPool[loadedPoolNames.Count];
            for (int i = 0; i != bufferPools.Length; i++)
            {
                using (BinaryReader reader = new BinaryReader(File.Open(loadedPoolNames[i].FullName, FileMode.Open)))
                    bufferPools[i] = new VertexBufferPool(reader);
            }
        }

        /// <summary>
        /// Get buffer from manager. use VertexBufferRef from FrameGeometry.
        /// </summary>
        /// <param name="vertexRef">vertexBufferRef</param>
        /// <returns></returns>
        public VertexBuffer GetBuffer(ulong vertexRef)
        {
            for (int i = 0; i != bufferPools.Length; i++)
            {
                foreach (KeyValuePair<ulong, VertexBuffer> entry in bufferPools[i].Buffers)
                {
                    if (entry.Key == vertexRef)
                        return entry.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// writer pools to their files.
        /// </summary>
        public void WriteToFile()
        {
            for (int i = 0; i != bufferPools.Length; i++)
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(loadedPoolNames[i].FullName, FileMode.Create)))
                    bufferPools[i].WriteToFile(writer);
            }
        }
    }

    public class VertexBufferPool
    {
        //MAX BUFFER SIZE IS 128
        BufferType version;
        int numBuffers;
        int size;
        Dictionary<ulong, VertexBuffer> buffers = new Dictionary<ulong, VertexBuffer>();

        public Dictionary<ulong, VertexBuffer> Buffers {
            get { return buffers; }
            set { buffers = value; }
        }

        /// <summary>
        /// Construct pool and read buffers.
        /// </summary>
        /// <param name="reader"></param>
        public VertexBufferPool(BinaryReader reader)
        {
            ReadFromFile(reader);
        }

        /// <summary>
        /// read all buffers from the file.
        /// </summary>
        /// <param name="reader"></param>
        public void ReadFromFile(BinaryReader reader)
        {
            version = (BufferType)reader.ReadByte();
            numBuffers = reader.ReadInt32();
            size = reader.ReadInt32();

            for (int i = 0; i != numBuffers; i++)
            {
                VertexBuffer buffer = new VertexBuffer(reader);
                buffers.Add(buffer.Hash, buffer);
            }
        }

        /// <summary>
        /// Write all buffers to the file.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteToFile(BinaryWriter writer)
        {
            writer.Write((byte)version);
            writer.Write(numBuffers);
            writer.Write(size);

            for (int i = 0; i != buffers.Count; i++)
                buffers.ElementAt(i).Value.WriteToFile(writer);
        }
    }

    public class VertexBuffer
    {
        ulong hash;
        int len;
        byte[] data;

        public ulong Hash {
            get { return hash; }
            set { hash = value; }
        }
        public byte[] Data {
            get { return data; }
            set {
                data = value;
                len = data.Length;
            }
        }

        /// <summary>
        /// Construct a buffer with given hash.
        /// </summary>
        /// <param name="hash"></param>
        public VertexBuffer(ulong hash)
        {
            this.hash = hash;
        }

        /// <summary>
        /// Construct pool and read buffers.
        /// </summary>
        /// <param name="reader"></param>
        public VertexBuffer(BinaryReader reader)
        {
            ReadFromFile(reader);
        }

        /// <summary>
        /// Read buffer to file.
        /// </summary>
        /// <param name="reader"></param>
        public void ReadFromFile(BinaryReader reader)
        {
            hash = reader.ReadUInt64();
            len = reader.ReadInt32();
            data = reader.ReadBytes(len);
        }

        /// <summary>
        /// Write buffer to file.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteToFile(BinaryWriter writer)
        {
            writer.Write(hash);
            writer.Write(len);
            writer.Write(data);
        }
    }
}

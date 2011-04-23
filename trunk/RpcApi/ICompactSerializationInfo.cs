using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;

namespace RpcApi
{
    [ComVisible(true)]
    interface ICompactSerializationInfo
    {
        //--- Public Methods ---

        public void AddValue(bool value);
        
        public void AddValue(byte value);
        
        public void AddValue(char value);
        
        public void AddValue(DateTime value);
        
        public void AddValue(decimal value);
        
        public void AddValue(double value);
        
        public void AddValue(short value);
        
        public void AddValue(int value);
        
        public void AddValue(long value);
        
        public void AddValue(object value);
        
        [CLSCompliant(false)]
        public void AddValue(sbyte value);
        
        public void AddValue(float value);
        
        [CLSCompliant(false)]
        public void AddValue(ushort value);
        
        [CLSCompliant(false)]
        public void AddValue(uint value);
        
        [CLSCompliant(false)]
        public void AddValue(ulong value);
        
        public void AddValue(object value, Type type);
        
        public bool GetBoolean();
        
        public byte GetByte();
        
        public char GetChar();
        
        public DateTime GetDateTime();
        
        public decimal GetDecimal();
        
        public double GetDouble();
        
        public short GetInt16();
        
        public int GetInt32();
        
        public long GetInt64();
        
        [CLSCompliant(false)]
        public sbyte GetSByte();
        
        public float GetSingle();
        
        public string GetString();
        
        [CLSCompliant(false)]
        public ushort GetUInt16();
        
        [CLSCompliant(false)]
        public uint GetUInt32();
        
        [CLSCompliant(false)]
        public ulong GetUInt64();
        
        [SecuritySafeCritical]
        public object GetValue(Type type);
    }
}

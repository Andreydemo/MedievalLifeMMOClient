﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NewCrypt
{

    private Logger _log = new Logger(new MyLogHandler());

    BlowfishEngine _crypt;
    BlowfishEngine _decrypt;

    /**
	 * @param blowfishKey
	 */
    public NewCrypt(byte[] blowfishKey)
    {
        _crypt = new BlowfishEngine();
        _crypt.init(true, blowfishKey);
        _decrypt = new BlowfishEngine();
        _decrypt.init(false, blowfishKey);
    }

    public NewCrypt(string key)
    {
        byte[] blowfishKey = Encoding.UTF8.GetBytes(key);
        _crypt = new BlowfishEngine();
        _crypt.init(true, blowfishKey);
        _decrypt = new BlowfishEngine();
        _decrypt.init(false, blowfishKey);
    }

    public static bool verifyChecksum(byte[] raw)
    {
        return NewCrypt.verifyChecksum(raw, 0, raw.Length);
    }

    public static bool verifyChecksum(byte[] raw, int offset, int size)
    {
        // check if size is multiple of 4 and if there is more then only the checksum
        if (((size & 3) != 0) || (size <= 4))
        {
            return false;
        }

        long chksum = 0;
        int count = size - 4;
        long check = -1;
        int i;

        for (i = offset; i < count; i += 4)
        {
            check = (uint)raw[i] & 0xff;
            check |= ((uint)raw[i + 1] << 8) & 0xff00;
            check |= ((uint)raw[i + 2] << 0x10) & 0xff0000;
            check |= ((uint)raw[i + 3] << 0x18) & 0xff000000;

            chksum ^= check;
        }

        check = (uint)raw[i] & 0xff;
        check |= ((uint)raw[i + 1] << 8) & 0xff00;
        check |= ((uint)raw[i + 2] << 0x10) & 0xff0000;
        check |= ((uint)raw[i + 3] << 0x18) & 0xff000000;

        return check == chksum;
    }

    public static void appendChecksum(byte[] raw)
    {
        NewCrypt.appendChecksum(raw, 0, raw.Length);
    }

    public static void appendChecksum(byte[] raw, int offset, int size)
    {
        long chksum = 0;
        int count = size - 4;
        long ecx;
        int i;

        for (i = offset; i < count; i += 4)
        {
            ecx = (uint)raw[i] & 0xff;
            ecx |= ((uint)raw[i + 1] << 8) & 0xff00;
            ecx |= ((uint)raw[i + 2] << 0x10) & 0xff0000;
            ecx |= ((uint)raw[i + 3] << 0x18) & 0xff000000;

            chksum ^= ecx;
        }

        ecx = (uint)raw[i] & 0xff;
        ecx |= ((uint)raw[i + 1] << 8) & 0xff00;
        ecx |= ((uint)raw[i + 2] << 0x10) & 0xff0000;
        ecx |= ((uint)raw[i + 3] << 0x18) & 0xff000000;

        raw[i] = (byte)(chksum & 0xff);
        raw[i + 1] = (byte)((chksum >> 0x08) & 0xff);
        raw[i + 2] = (byte)((chksum >> 0x10) & 0xff);
        raw[i + 3] = (byte)((chksum >> 0x18) & 0xff);
    }

    /**
	 * Packet is first XOR encoded with <code>key</code> Then, the last 4 bytes are overwritten with the the XOR "key". Thus this assume that there is enough room for the key to fit without overwriting data.
	 * @param raw The raw bytes to be encrypted
	 * @param key The 4 bytes (int) XOR key
	 */
    public static void encXORPass(byte[] raw, int key)
    {
        NewCrypt.encXORPass(raw, 0, raw.Length, key);
    }

    /**
	 * Packet is first XOR encoded with <code>key</code> Then, the last 4 bytes are overwritten with the the XOR "key". Thus this assume that there is enough room for the key to fit without overwriting data.
	 * @param raw The raw bytes to be encrypted
	 * @param offset The begining of the data to be encrypted
	 * @param size Length of the data to be encrypted
	 * @param key The 4 bytes (int) XOR key
	 */
    public static void encXORPass(byte[] raw, int offset, int size, int key)
    {
        int stop = size - 8;
        int pos = 4 + offset;
        int edx;
        int ecx = key; // Initial xor key

        while (pos < stop)
        {
            edx = (raw[pos] & 0xFF);
            edx |= (raw[pos + 1] & 0xFF) << 8;
            edx |= (raw[pos + 2] & 0xFF) << 16;
            edx |= (raw[pos + 3] & 0xFF) << 24;

            ecx += edx;

            edx ^= ecx;

            raw[pos++] = (byte)(edx & 0xFF);
            raw[pos++] = (byte)((edx >> 8) & 0xFF);
            raw[pos++] = (byte)((edx >> 16) & 0xFF);
            raw[pos++] = (byte)((edx >> 24) & 0xFF);
        }

        raw[pos++] = (byte)(ecx & 0xFF);
        raw[pos++] = (byte)((ecx >> 8) & 0xFF);
        raw[pos++] = (byte)((ecx >> 16) & 0xFF);
        raw[pos++] = (byte)((ecx >> 24) & 0xFF);
    }

    public byte[] decrypt(byte[] raw)
    {

        byte[] result = new byte[raw.Length];
        int count = raw.Length / 8;

        for (int i = 0; i < count; i++)
        {
            _decrypt.processBlock(raw, i * 8, result, i * 8);
        }

        return result;
    }

    public void decrypt(byte[] raw, int offset, int size)
    {
        byte[]
    result = new byte[size];
        int count = size / 8;

        for (int i = 0; i < count; i++)
        {
            _decrypt.processBlock(raw, offset + (i * 8), result, i * 8);
        }
        // TODO can the crypt and decrypt go direct to the array
        Array.Copy(result, 0, raw, offset, size);
    }

    public byte[] crypt(byte[] raw)
    {
        int count = raw.Length / 8;
        byte[]
    result = new byte[raw.Length];

        for (int i = 0; i < count; i++)
        {
            _crypt.processBlock(raw, i * 8, result, i * 8);
        }

        return result;
    }

    public void crypt(byte[] raw, int offset, int size)
    {
        int count = size / 8;
        byte[]
    result = new byte[size];

        for (int i = 0; i < count; i++)
        {
            _crypt.processBlock(raw, offset + (i * 8), result, i * 8);
        }
        // TODO can the crypt and decrypt go direct to the array
        Array.Copy(result, 0, raw, offset, size);
    }
}

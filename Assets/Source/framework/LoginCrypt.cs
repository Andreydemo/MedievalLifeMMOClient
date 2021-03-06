﻿using System;
using System.Collections;
using System.Collections.Generic;

public class LoginCrypt
{
    Random rnd = new Random();
    private static byte[] STATIC_BLOWFISH_KEY =
    {
        (byte) 0x6b,
        (byte) 0x60,
        (byte) 0xcb,
        (byte) 0x5b,
        (byte) 0x82,
        (byte) 0xce,
        (byte) 0x90,
        (byte) 0xb1,
        (byte) 0xcc,
        (byte) 0x2b,
        (byte) 0x6c,
        (byte) 0x55,
        (byte) 0x6c,
        (byte) 0x6c,
        (byte) 0x6c,
        (byte) 0x6c
    };

    private NewCrypt _staticCrypt = new NewCrypt(STATIC_BLOWFISH_KEY);
    private NewCrypt _crypt;
    private bool _static = true;

    public void setKey(byte[] key)
    {
        _crypt = new NewCrypt(key);
    }

    public bool decrypt(byte[] raw, int offset, int size)
    {
        _crypt.decrypt(raw, offset, size);
        return NewCrypt.verifyChecksum(raw, offset, size);
    }

    public int encrypt(byte[] raw, int offset, int size)
    {
        // reserve checksum
        size += 4;

        if (_static)

        {
            // reserve for XOR "key"
            size += 4;

            // padding
            size += 8 - (size % 8);
            NewCrypt.encXORPass(raw, offset, size, rnd.Next());
            _staticCrypt.crypt(raw, offset, size);

            _static = false;
        }
        else

        {
            // padding
            size += 8 - (size % 8);
            NewCrypt.appendChecksum(raw, offset, size);
            _crypt.crypt(raw, offset, size);
        }
        return size;
    }
}

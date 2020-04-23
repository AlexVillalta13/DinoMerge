using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;
using System.IO;
using System.Text;

namespace DemiumGames.Saveable{

public class CypherRSA
{
	CspParameters cspp;
	RSACryptoServiceProvider rsa;

	// Use this for initialization

	public CypherRSA (string keyname)
	{
		cspp = new CspParameters (); 
		cspp.KeyContainerName = keyname; 

		rsa = new RSACryptoServiceProvider (cspp); 
		rsa.PersistKeyInCsp = true; 

		}



	public byte[] Encrypt (byte[] bytes)
	{
		


		byte[] bytesReturn = null; 
		RijndaelManaged rjndl = new RijndaelManaged (); 
		rjndl.KeySize = 256; 
		rjndl.BlockSize = 256; 
		rjndl.Mode = CipherMode.CBC; 

		ICryptoTransform transform = rjndl.CreateEncryptor (); 

		byte[] keyEncrypted = rsa.Encrypt (rjndl.Key, false); 

		byte[] LenK = new byte[4];
		byte[] LenIV = new byte[4];

		int lKey = keyEncrypted.Length; 
		LenK = BitConverter.GetBytes (lKey);
		int lIV = rjndl.IV.Length; 
		LenIV = BitConverter.GetBytes (lIV); 

		using (MemoryStream outFs = new MemoryStream ()) {
			outFs.Write (LenK, 0, 4); 
			outFs.Write (LenIV, 0, 4); 
			outFs.Write (keyEncrypted, 0, lKey); 
			outFs.Write (rjndl.IV, 0, lIV); 
			using (CryptoStream outStreamEncrypted = new CryptoStream (outFs, transform, CryptoStreamMode.Write)) {


				outStreamEncrypted.Write (bytes, 0, bytes.Length); 
				outStreamEncrypted.FlushFinalBlock ();
				outStreamEncrypted.Close (); 
			}
			outFs.Close (); 
			bytesReturn = outFs.ToArray (); 

		}
		return bytesReturn; 

	}





	public byte[] Decrypt (Stream stream)
	{

		byte[] byteArr = null; 
		RijndaelManaged rjndl = new RijndaelManaged ();
		rjndl.KeySize = 256;
		rjndl.BlockSize = 256;
		rjndl.Mode = CipherMode.CBC;


		byte[] LenK = new byte[4];
		byte[] LenIV = new byte[4];




		stream.Seek (0, SeekOrigin.Begin);
		stream.Seek (0, SeekOrigin.Begin);
		stream.Read (LenK, 0, 3);
		stream.Seek (4, SeekOrigin.Begin);
		stream.Read (LenIV, 0, 3);

		int lenK = BitConverter.ToInt32 (LenK, 0);
		int lenIV = BitConverter.ToInt32 (LenIV, 0);


		int startC = lenK + lenIV + 8;


		byte[] KeyEncrypted = new byte[lenK];
		byte[] IV = new byte[lenIV];


		stream.Seek (8, SeekOrigin.Begin);
		stream.Read (KeyEncrypted, 0, lenK);
		stream.Seek (8 + lenK, SeekOrigin.Begin);
		stream.Read (IV, 0, lenIV);

		byte[] KeyDecrypted = rsa.Decrypt (KeyEncrypted, false);

		ICryptoTransform transform = rjndl.CreateDecryptor (KeyDecrypted, IV);


		using (MemoryStream outFs = new MemoryStream ()) {

			int count = 0;
			int offset = 0;

			// blockSizeBytes can be any arbitrary size.
			int blockSizeBytes = rjndl.BlockSize / 8;
			byte[] data = new byte[blockSizeBytes];

			stream.Seek (startC, SeekOrigin.Begin);
			using (CryptoStream outStreamDecrypted = new CryptoStream (outFs, transform, CryptoStreamMode.Write)) {
				do {
					count = stream.Read (data, 0, blockSizeBytes);
					offset += count;
					outStreamDecrypted.Write (data, 0, count);

				} while (count > 0);

				outStreamDecrypted.FlushFinalBlock ();
				outStreamDecrypted.Close ();
			}
			outFs.Close ();
			byteArr = outFs.ToArray (); 
		}
		stream.Close ();

		return byteArr; 
	}
		

	

}
}
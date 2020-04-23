using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System;

namespace DemiumGames.Saveable
{
    public static class JsonFormatterHelper
    {
        public delegate void CallBackJson<T>(T obj);

        public static void Save<T>(T obj, string path, string name, bool secure = false)
        {

            string json = JsonUtility.ToJson(obj, true);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            if (secure)
            {
                String hash = GetHash(bytes);
                bytes = GetJsonWithHash(json, hash); 
            }

            SaveToFile(bytes, path, name);

        }

        private static byte[] GetJsonWithHash(string json, string hash){
            string aux = String.Empty;
			for (int i = 0; i < json.Length; i++)
			{
				aux += json[i];
				if (json.Length - 3 == i)
				{
					aux += ",";
				}
				if (json.Length - 2 == i)
				{
                    aux += AddHashToFinalJson(hash);
				}
			}
            return Encoding.UTF8.GetBytes(aux); 
		}

        private static string AddHashToFinalJson(string hash){
            string aux = string.Empty;
			aux += "\t";
			aux += "\"hash\"" + ":" + "\"" + hash + "\"";
			aux += Environment.NewLine;
			return aux; 
        }


		public static string GetHash(byte[] bytes)
		{
			SHA256Managed hashSHA = new SHA256Managed();
			byte[] cryptedBytes = hashSHA.ComputeHash(bytes);

			StringBuilder hash = new StringBuilder();


			for (int i = 0; i < cryptedBytes.Length; i++)
			{
				hash.Append(cryptedBytes[i].ToString("x2"));
			}

			return hash.ToString();
		}

        private static void SaveToFile(byte[] bytes, string path, string name){
			FileStream file = new FileStream(path + "/" + name, FileMode.Create);
			file.Write(bytes, 0, bytes.Length);
			file.Close();
        }

        public static T Load<T>(string json, bool secure = false)
        {
            return GetObjectFromByteArray<T>(Encoding.UTF8.GetBytes(json), true);
        }

        public static T Load<T>(string path, string name, bool secure = false)
        {
            T aux = default(T);
            if (File.Exists(path + "/" + name))
            {

                byte[] bytes = File.ReadAllBytes(path + "/" + name);
                aux = GetObjectFromByteArray<T>(bytes, secure);
            }
            return aux;
        }


        private static T GetObjectFromByteArray<T>(byte[] bytes, bool secure)
        {
            string originalJson = Encoding.UTF8.GetString(bytes);
            T aux = JsonUtility.FromJson<T>(originalJson);

            if (secure && GetFileIsTampered(originalJson, aux))
            {
                Debug.Log("Save file tampered, please restrain from editing the save file");
                return default(T);
            }

            return aux;
        }

        private static bool GetFileIsTampered<T>(string originalJson, T aux){
			string jsonHash = GetHashNode(originalJson);
			string partialJson = JsonUtility.ToJson(aux, true);
			string partialHash = GetHash(Encoding.UTF8.GetBytes(partialJson));

            return !partialHash.Equals(jsonHash); 
        }


        public static void LoadFromJSON<T>(this MonoBehaviour obj, string url, CallBackJson<T> call, bool secure = false)
        {
            obj.StartCoroutine(LoadCoroutine<T>(url, call, secure));
        }

        private static IEnumerator LoadCoroutine<T>(string url, CallBackJson<T> call, bool secure)
        {

            WWW www = new WWW(url);
            yield return www;

            if (www.error == null)
            {
                SuccesfullLoad<T>(www.bytes, call, secure);
                www.Dispose();
                yield break;

            }
            else
            {
                www.Dispose();
                yield break;
            }
        }


        private static void SuccesfullLoad<T>(byte[] byteArr, CallBackJson<T> call, bool secure){
			T obj = GetObjectFromByteArray<T>(byteArr, secure);
			call(obj);
        }



      

        private static string GetHashNode(string json)
        {

            int i = GetJsonIndexToEndOfHash(json);
            string aux = GetReversedHashFromJson(json, i);
            string hash = ReverseString(aux);

            return hash;
        }

        private static int GetJsonIndexToEndOfHash(string json){
			int i = json.Length - 1;

			while (json[i] != '"')
			{
				i--;
			}
			i--;
            return i; 
        }

        private static string GetReversedHashFromJson(string json, int index){
            int i = index;
            string aux = String.Empty;
			while (json[i] != '"')
			{
				aux += json[i];
				i--;
			}
            return aux; 
		}

        private static string ReverseString(string aux){
            String result = String.Empty; 
            for (int i = aux.Length - 1; i >= 0; i--)
            {
                result += aux[i];
            }
            return result; 
        }

    }

    public class JsonFormatterObject
    {
        public JsonFormatterObject()
        {

        }

        public T Load<T>(string path, string name, bool secure = false)
        {
            return JsonFormatterHelper.Load<T>(path, name, secure);

        }
    }
}
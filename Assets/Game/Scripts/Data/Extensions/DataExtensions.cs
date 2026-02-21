using UnityEngine;

namespace Game.Scripts.Data.Extensions
{
    public static class DataExtensions
    {
        public static Vector3Data AsVector3Data(this Vector3 vector) =>
            new Vector3Data(vector.x, vector.y, vector.z);

        public static Vector3 AsVector3(this Vector3Data vector3Data) =>
            new Vector3(vector3Data.X, vector3Data.Y, vector3Data.Z);

        public static T ToDeserializes<T>(this string serializedString) =>
            JsonUtility.FromJson<T>(serializedString);
        public static string ToJson(this object obj) =>
            JsonUtility.ToJson(obj);
        
        public static Vector3 AddY(this Vector3 vector, float y) 
        {
            vector.y += y;
            return vector;
        }
    }
}
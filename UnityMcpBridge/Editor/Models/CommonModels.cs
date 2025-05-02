using Newtonsoft.Json;
using UnityEngine;

namespace UnityMcp.Editor.Models
{
    public class BaseActionRequest
    {
        public string action { get; set; }

        public string id { get; set; }
    }

    public class BaseActionResponse
    {
        public string id { get; set; }
        public string action { get; set; }
        public bool success { get; set; } = false;
        public string message { get; set; }

        public BaseActionResponse Fail(BaseActionRequest request, string message)
        {
            id = request.id;
            action = request.action;
            success = false;
            this.message = message;
            return this;
        }

        public BaseActionResponse Success(BaseActionRequest request, string message = null)
        {
            id = request.id;
            action = request.action;
            success = true;
            this.message = message;
            return this;
        }
    }

    public class TransformData
    {
        public Vector3Data position { get; set; }

        public Vector3Data rotation { get; set; }

        public Vector3Data world_position { get; set; }

        public Vector3Data world_rotation { get; set; }

        public Vector3Data scale { get; set; }
    }

    public class Vector3Data
    {
        public float x { get; set; }

        public float y { get; set; }

        public float z { get; set; }

        public static Vector3Data FromVector3(Vector3 vector)
        {
            return new Vector3Data
            {
                x = vector.x,
                y = vector.y,
                z = vector.z
            };
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}

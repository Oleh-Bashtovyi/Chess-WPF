using Chess_game.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Extensions
{
    public static class Json_Extension
    {

        // Try get int
        public static bool TryGet_IntValue(this JToken jsonToken, string key, out int variable)
        {
            JToken? token = jsonToken[key];

            if (token != null)
            {
                variable = Convert.ToInt32(token);
                return true;
            }

            variable = 0;
            return false;
        }
        public static bool TryGet_IntValue(this JObject jsonObject, string key, out int variable)
        {
            JToken? token = jsonObject[key];

            if (token != null)
            {
                variable = Convert.ToInt32(token);
                return true;
            }

            variable = 0;
            return false;
        }

        public static bool TryGet_ShortValue(this JObject jsonObject, string key, out short variable)
        {
            JToken? token = jsonObject[key];

            if (token != null)
            {
                variable = Convert.ToInt16(token);
                return true;
            }

            variable = 0;
            return false;
        }

        // Try  get enum
        public static bool TryGet_EnumValue<TEnum>(this JToken jsonToken, string key, out TEnum? variable) where TEnum : Enum
        {
            JToken? token = jsonToken[key];

            if (token != null)
            {
                if(Enum.TryParse(typeof(TEnum), token.ToString(), out object? result))
                {
                    variable = (TEnum) result;
                    return true;
                }
            }

            variable = default;
            return false;
        }
        public static bool TryGet_EnumValue<TEnum>(this JObject jsonObject, string key, out TEnum? variable) where TEnum : Enum
        {
            JToken? token = jsonObject[key];

            if (token != null)
            {
                if (Enum.TryParse(typeof(TEnum), token.ToString(), out object? result))
                {
                    variable = (TEnum)result;
                    return true;
                }
            }

            variable = default;
            return false;
        }


        // Get boolean
        public static bool Get_BoolValue(this JToken jsonToken, string key)
        {
            JToken? token = jsonToken[key];

            if (token != null)
            {
                return Convert.ToBoolean(token);
            }
            return false;
        }
        public static bool Get_BoolValue(this JObject jsonObject, string key)
        {
            JToken? token = jsonObject[key];

            if (token != null)
            {
                return Convert.ToBoolean(token);
            }
            return false;
        }


        // Try get boolean
        public static bool TryGet_BoolValue(this JToken jsonToken, string key, out bool result)
        {
            JToken? token = jsonToken[key];

            if (token != null)
            {
                bool.TryParse(token.ToString(), out result);
                return true;
            }
            result= false;
            return false;
        }
        public static bool TryGet_BoolValue(this JObject jsonObject, string key, out bool result)
        {
            JToken? token = jsonObject[key];

            if (token != null)
            {
                bool.TryParse(token.ToString(), out result);
                return true;
            }
            result = false;
            return false;
        }

         



        // Get int
        public static int Get_IntValue(this JToken jsonToken, string key)
        {
            JToken? token = jsonToken[key];

            if (token != null)
            {
                return Convert.ToInt32(token);
            }
            return 0;
        }

        public static string Get_StringValue(this JToken jsonToken, string key)
        {
            JToken? token = jsonToken[key];

            if (token != null)
            {
                return token.ToString();
            }
            return string.Empty;
        }



        public static bool TryGet_StringValue(this JObject jsonObject, string key, out string result)
        {
            JToken? token = jsonObject[key];

            if (token != null)
            {
                result = token.ToString();
                return true;
            }
            
            result = string.Empty;
            return false;
        }

    }
}

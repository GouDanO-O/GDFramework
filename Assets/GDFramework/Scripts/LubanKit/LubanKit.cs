using GDFramework.LubanKit.Cfg;
using GDFrameworkCore;
using GDFrameworkExtend.FluentAPI;
using GDFrameworkExtend.ResKit;
using Luban;
using SimpleJSON;
using UnityEngine;

namespace GDFramework.LubanKit
{
    public class LubanKit : AbstractSystem
    {
        public LubanTablesData LubanTablesData;
        
        protected override void OnInit()
        {
            
        }

        public void InitData()
        {
            var tablesCtor = typeof(LubanTablesData).GetConstructors()[0];
            var loaderReturnType = tablesCtor.GetParameters()[0].ParameterType.GetGenericArguments()[1];
            System.Delegate loader = loaderReturnType == typeof(ByteBuf) ?
                new System.Func<string, ByteBuf>(LoadByteBuf)
                : (System.Delegate)new System.Func<string, JSONNode>(LoadJson);
            LubanTablesData = (LubanTablesData)tablesCtor.Invoke(new object[] { loader });
        }
        
        /// <summary>
        /// 加载二进制格式的配置文件
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private ByteBuf LoadByteBuf(string files)
        {
            byte[] jsons = null;
            var _mloader = ResLoader.Allocate();
            _mloader.Add2Load<TextAsset>($"yoo:{files}", (a, res) =>
            {

                if (a)
                {
                    jsons = res.Asset.As<TextAsset>().bytes;
                }
            });
            _mloader.LoadAsync();
            _mloader.Recycle2Cache();
            _mloader = null;
            return new ByteBuf(jsons);
        }
        
        /// <summary>
        /// 加载json格式的配置文件
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private JSONNode LoadJson(string files)
        {
            string jsons = null;
            var _mloader = ResLoader.Allocate();
            _mloader.Add2Load<TextAsset>($"yoo:{files}", (a, res) =>
            {
                if (a)
                {
                    jsons = res.Asset.As<TextAsset>().text;
                }
            });
            _mloader.LoadAsync();
            _mloader.Recycle2Cache();
            _mloader = null;
            return JSON.Parse(jsons);

        }
    }
}
using SyZero;

namespace SyZero.AI.Web.Models
{
    public class ResultModel
    {
        /// <summary>
        /// 自定义
        /// </summary>
        /// <param name="data">数据对象 例如:{status=500,action="6666"}</param>
        /// <param name="msgStatus"></param>
        public ResultModel(object Data)
        {
            this.code = (int)SyMessageBoxStatus.Success;
            this.data = Data;
        }

        /// <summary>
        /// 成功状态码
        /// </summary>
        public int code { set; get; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public object data { set; get; }
    }
}




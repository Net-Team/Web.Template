namespace Core.Masks
{
    /// <summary>
    /// 表示掩码电话号码
    /// </summary>
    public class MaskPhoneNo : MaskString
    {
        /// <summary>
        /// 掩码电话号码
        /// </summary>
        /// <param name="phoneNumber">电话号码</param>
        public MaskPhoneNo(string phoneNumber)
            : base(phoneNumber, 3, 4)
        {
        }

        /// <summary>
        /// 从string类型的电话号码显式转换得到
        /// </summary>
        /// <param name="phoneNumber"></param>
        public static explicit operator MaskPhoneNo(string phoneNumber)
        {
            return new MaskPhoneNo(phoneNumber);
        }
    }
}

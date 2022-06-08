using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

namespace GacArduinoHelper
{
    /// <summary>
    /// ToolTip美化扩展
    /// </summary>
    [ToolboxItem(true)]
    [Description("ToolTip美化扩展-参考")]
    [DefaultProperty("TitleShow")]
    [DefaultEvent("Popup")]
    public partial class GacToolTip : ToolTip
    {
        #region 新增属性

        private ToolTipAnchor toolAnchor = ToolTipAnchor.TopCenter;
        /// <summary>
        /// 提示框位置
        /// </summary>
        [Browsable(false)]
        [DefaultValue(ToolTipAnchor.TopCenter)]
        [Description("提示框位置")]
        public ToolTipAnchor ToolAnchor
        {
            get { return this.toolAnchor; }
            set
            {
                if (this.toolAnchor == value)
                    return;
                this.toolAnchor = value;
            }
        }

        private int anchorDistance = 3;
        /// <summary>
        /// 提示框位置距离
        /// </summary>
        [DefaultValue(20)]
        [Description("提示框位置距离")]
        public int AnchorDistance
        {
            get { return this.anchorDistance; }
            set
            {
                if (this.anchorDistance == value || value < 0)
                    return;
                this.anchorDistance = value;
            }
        }

        private int padding = 3;
        /// <summary>
        /// 内边距
        /// </summary>
        [DefaultValue(3)]
        [Description("内边距")]
        public int Padding
        {
            get { return this.padding; }
            set
            {
                if (this.padding == value || value < 0)
                    return;
                this.padding = value;
            }
        }

        private Size minSize = new Size(20, 10);
        /// <summary>
        /// 内容最小大小
        /// </summary>
        [DefaultValue(typeof(Size), "20,10")]
        [Description("内容最小大小")]
        public Size MinSize
        {
            get { return this.minSize; }
            set
            {
                if (this.minSize == value || value.Width < 0 || value.Height < 0)
                    return;
                this.minSize = value;
            }
        }

        private Size maxSize = new Size(0, 0);
        /// <summary>
        /// 内容最大大小
        /// </summary>
        [DefaultValue(typeof(Size), "0,0")]
        [Description("内容最大大小")]
        public Size MaxSize
        {
            get { return this.maxSize; }
            set
            {
                if (this.maxSize == value || value.Width < 0 || value.Height < 0)
                    return;
                this.maxSize = value;
            }
        }



        private TitleAnchor titleStation = TitleAnchor.Top;
        /// <summary>
        /// 提示框标题位置
        /// </summary>
        [DefaultValue(TitleAnchor.Top)]
        [Description("提示框标题位置")]
        public TitleAnchor TitleStation
        {
            get { return this.titleStation; }
            set
            {
                if (this.titleStation == value)
                    return;
                this.titleStation = value;
            }
        }


        private Member _Member= new Member();
        /// <summary>
        /// 参数信息
        /// </summary>
        [Description("要提示的参数信息")]
        public Member Member
        {
            get { return this._Member; }
            set
            {
                if (this._Member == value)
                    return;
                this._Member = value;
            }
        }



        #region 文本

        private Font font = new Font("宋体", 10);
        /// <summary>
        /// 文本字体
        /// </summary>
        [DefaultValue(typeof(Font), "宋体, 10pt")]
        [Description("文本字体")]
        public Font Font
        {
            get { return this.font; }
            set
            {
                if (this.font == value)
                    return;
                this.font = value;
            }
        }

        private Color _ColorDataType = Color.Blue;
        /// <summary>
        /// 参数类型颜色
        /// </summary>
        [Description("参数类型颜色")]
        public Color ColorDataType
        {
            get { return this._ColorDataType; }
            set
            {
                if (this._ColorDataType == value)
                    return;
                this._ColorDataType = value;
            }
        }

        //private Color _ColorValueNumer = Color.Orange;
        ///// <summary>
        ///// 参数类型颜色
        ///// </summary>
        //[Description("参数值数字颜色")]
        //public Color ColorValueNumer
        //{
        //    get { return this._ColorValueNumer; }
        //    set
        //    {
        //        if (this._ColorValueNumer == value)
        //            return;
        //        this._ColorValueNumer = value;
        //    }
        //}

        private Color _ColorValueText = Color.Orange;
        /// <summary>
        /// 参数值字符串颜色
        /// </summary>
        [Description("参数值字符串颜色")]
        public Color ColorValueText
        {
            get { return this._ColorValueText; }
            set
            {
                if (this._ColorValueText == value)
                    return;
                this._ColorValueText = value;
            }
        }


        #endregion

        #endregion

        public GacToolTip()
        {
            this.OwnerDraw = true;
            //this.BackColor =Color.ba
            //this.ForeColor = Color.FromArgb(245, 168, 154);
            this.Popup += new PopupEventHandler(this.ToolTipExt_Popup);
            this.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.ToolTipExt_Draw);
        }

        #region 重写

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        #region 公开方法
        string MethodText = "";
        string MethodParam = "";
       
        /// <summary>
        /// 设置与指定控件关联的工具提示文本，然后在指定的相对位置以模式方式显示该工具提示。
        /// </summary>
        /// <param name="text">包含新工具提示文本的 System.String。</param>
        /// <param name="window">要为其显示工具提示的 System.Windows.Forms.Control。</param>
        /// <param name="rect">重新定义关联控件窗口的rect信息</param>
        /// <param name="anchor">工具提示的位置</param>
        /// <param name="duration">包含工具提示持续显示时间（以毫秒为单位）的 System.Int32。</param>
        public void Show(string text, IWin32Window window, Rectangle rect, ToolTipAnchor anchor, int duration)
        {
            Member = null;
            this.toolAnchor = anchor;
            Control control = (Control)window;
            if (rect == Rectangle.Empty)
            {
                rect.X = control.Location.X;
                rect.Y = control.Location.Y;
                rect.Width = control.Width;
                rect.Height = control.Height;
            }

            Size size = this.GetToolTipSize(control, text);
            Point point = new Point(0, 0);
            if (this.ToolAnchor == ToolTipAnchor.TopCenter)
            {
                point = new Point(rect.X + (rect.Width - size.Width) / 2, rect.Y - this.AnchorDistance - size.Height);
            }
            else if (this.ToolAnchor == ToolTipAnchor.BottomCenter)
            {
                point = new Point(rect.X + (rect.Width - size.Width) / 2, rect.Bottom + this.AnchorDistance);
            }
            else if (this.ToolAnchor == ToolTipAnchor.LeftCenter)
            {
                point = new Point(rect.X - (size.Width + this.AnchorDistance), rect.Y + (rect.Height - size.Height) / 2);
            }
            else if (this.ToolAnchor == ToolTipAnchor.RightCenter)
            {
                point = new Point(rect.Right + this.AnchorDistance, rect.Y + (rect.Height - size.Height) / 2);
            }
            this.Show(text, window, point, duration);
        }


        public void Show(Member member, IWin32Window window,Point point)
        {
            this.Member = member;
            Control control = (Control)window;
            Rectangle rect = new Rectangle();
            if (rect == Rectangle.Empty)
            {
                rect.Width = control.Width;
                rect.Height = control.Height;
            }
            MethodParam = "";
            for (int i = 0; i < member.Param.Count; i++)
            {
                if (!string.IsNullOrEmpty(MethodParam))
                {
                    MethodParam += ",";
                }
                MethodParam += (member.Param[i].DataType==""?"": member.Param[i].DataType + " ") + member.Param[i].Name + (member.Param[i].Default == "" ? "" :" = "+ member.Param[i].Default);
            }
            MethodText =(string.IsNullOrEmpty(member.returns.DataType)?"": member.returns.DataType+" ") +member.Name + "("+ MethodParam + ")";
            string s = MethodText;
            s += string.IsNullOrEmpty(member.summary)?"":"\r\n"+member.summary;
            s += GetParams();
            Size size = this.GetToolTipSize(control, s);
            if (point.X==0)
            {
                point = new Point(rect.Right + this.AnchorDistance, rect.Y);
            }
            this.Show(s, window, point);
        }
        public void ShowTip(string Value, IWin32Window window)
        {
            Member = null;
            if (string.IsNullOrEmpty(Value))
            {
                return;
            }
            Control control = (Control)window;
            Rectangle rect = new Rectangle();
            if (rect == Rectangle.Empty)
            {
                rect.Width = control.Width;
                rect.Height = control.Height;
            }
            Size size = this.GetToolTipSize(control, Value);
            Point  point = new Point(rect.Right + this.AnchorDistance, rect.Y);
            this.Show(Value, window, point);
        }
        public void ShowTip(string Value, IWin32Window window,Point point)
        {
            Member = null;
            if (string.IsNullOrEmpty(Value))
            {
                return ;
            }   
            Control control = (Control)window;
            Rectangle rect = new Rectangle();
            if (rect == Rectangle.Empty)
            {
                rect.Width = control.Width;
                rect.Height = control.Height;
            }
            Size size = this.GetToolTipSize(control, Value);
            if (point.X == 0)
            {
                point = new Point(rect.Right + this.AnchorDistance, rect.Y);
            }
            this.Show(Value, window, point);
        }

        #endregion

        #region 私有方法
        private string GetParams()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Member.Param)
            {
                if (!string.IsNullOrEmpty(item.Name)&&!string.IsNullOrEmpty(item.Desc))
                {
                    sb.Append("\r\n" + item.Name + "  " + item.Default + "  " +item.DataType + " : " + item.Desc);
                }
                
            }
            return sb.ToString();
        }



        private void ToolTipExt_Popup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = this.GetToolTipSize(e.AssociatedControl);
        }

        private void ToolTipExt_Draw(object sender, DrawToolTipEventArgs e)
        {
            #region 背景

            SolidBrush back_sb = new SolidBrush(this.BackColor);
            e.Graphics.FillRectangle(back_sb, e.Bounds);
            back_sb.Dispose();

            #endregion

            Rectangle titleback_rect = new Rectangle();

            Rectangle text_rect = new Rectangle();
            if (this.TitleStation == TitleAnchor.Top)
            {
                text_rect = new Rectangle(e.Bounds.X + this.Padding, titleback_rect.Bottom + this.Padding, e.Bounds.Width, e.Bounds.Height - titleback_rect.Height);
            }
            else if (this.TitleStation == TitleAnchor.Bottom)
            {
                text_rect = new Rectangle(e.Bounds.X + this.Padding, e.Bounds.Y + this.Padding, e.Bounds.Width, e.Bounds.Height - titleback_rect.Height);
            }
            else if (this.TitleStation == TitleAnchor.Left)
            {
                text_rect = new Rectangle(e.Bounds.X + titleback_rect.Width + this.Padding, e.Bounds.Y + this.Padding, e.Bounds.Width, e.Bounds.Height - titleback_rect.Height);
            }
            else if (this.TitleStation == TitleAnchor.Right)
            {
                text_rect = new Rectangle(e.Bounds.X + this.Padding, e.Bounds.Y + this.Padding, e.Bounds.Width, e.Bounds.Height - titleback_rect.Height);
            }
            #region 内容
            if (Member != null)
            {
                Size text_size = TextRenderer.MeasureText(e.Graphics, MethodText + GetParams(), Font, new Size(), TextFormatFlags.NoPadding);


                SolidBrush text_color = new SolidBrush(this.ForeColor);
                SolidBrush text_datatype = new SolidBrush(this.ColorDataType);
                SolidBrush text_string = new SolidBrush(this.ColorValueText);
                //Font font_br = new Font("Console", this.Font.Size);
                //string strtemp = ""; ;

                string strreturn = (string.IsNullOrEmpty(Member.returns.DataType) ? "" : Member.returns.DataType + " ");
                if (!string.IsNullOrEmpty(strreturn))
                {
                    e.Graphics.DrawString(strreturn, this.Font, text_datatype, text_rect);
                    text_rect.X += TextRenderer.MeasureText(e.Graphics, strreturn, Font, new Size(), TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width;
                }
                e.Graphics.DrawString(Member.Name, this.Font, text_color, text_rect);
                text_rect.X += TextRenderer.MeasureText(e.Graphics, Member.Name, Font, new Size(), TextFormatFlags.NoPadding).Width;
                if (!Member.isAttr)
                {
                    e.Graphics.DrawString("(", this.Font, text_color, text_rect);
                    text_rect.X += TextRenderer.MeasureText(e.Graphics, "(", Font, new Size(), TextFormatFlags.NoPadding).Width;
                }
                for (int i = 0; i < Member.Param.Count; i++)
                {
                    string datatype = (Member.Param[i].DataType == "" ? "" : Member.Param[i].DataType + " ");
                    e.Graphics.DrawString(datatype, this.Font, text_datatype, text_rect);
                    text_rect.X += TextRenderer.MeasureText(e.Graphics, datatype, Font, new Size(), TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width;


                    e.Graphics.DrawString(Member.Param[i].Name, this.Font, text_color, text_rect);
                    text_rect.X += TextRenderer.MeasureText(e.Graphics, Member.Param[i].Name, Font, new Size(), TextFormatFlags.NoPadding).Width;


                    if (!string.IsNullOrEmpty(Member.Param[i].Default))
                    {
                        e.Graphics.DrawString(" = ", this.Font, text_color, text_rect);
                        text_rect.X += TextRenderer.MeasureText(e.Graphics, " = ", Font, new Size(), TextFormatFlags.NoPadding).Width;
                        if (Member.Param[i].Default.StartsWith("\"") || Member.Param[i].Default.StartsWith("'"))
                        {

                            e.Graphics.DrawString(Member.Param[i].Default, this.Font, text_string, text_rect);
                        }
                        else
                        {
                            e.Graphics.DrawString(Member.Param[i].Default, this.Font, text_color, text_rect);
                        }
                        text_rect.X += TextRenderer.MeasureText(e.Graphics, Member.Param[i].Default, Font, new Size(), TextFormatFlags.NoPadding).Width;
                    }

                    if (i != Member.Param.Count - 1)
                    {
                        e.Graphics.DrawString(",", this.Font, text_color, text_rect);
                        text_rect.X += TextRenderer.MeasureText(e.Graphics, ",", Font, new Size(), TextFormatFlags.NoPadding).Width;
                    }



                }
                if (!Member.isAttr)
                {
                    e.Graphics.DrawString(")", this.Font, text_color, text_rect);
                }

                text_rect.X = this.Padding;
                //e.Graphics.DrawString(MethodText, this.Font, text_sb, text_rect);
                text_rect.Y += e.Graphics.MeasureString(MethodText, this.Font, new Size()).ToSize().Height;

                Font paramFont = new Font(Font.Name, Font.Size, FontStyle.Bold);
                Rectangle rect_param = new Rectangle(text_rect.Location, text_rect.Size);


                if (!string.IsNullOrEmpty(Member.summary))
                {
                    e.Graphics.DrawString(Member.summary, Font, text_color, text_rect);
                    rect_param.Y += e.Graphics.MeasureString(Member.summary, this.Font, new Size()).ToSize().Height;
                }
                for (int i = 0; i < Member.Param.Count; i++)
                {
                    if (!string.IsNullOrEmpty(Member.Param[i].Desc))
                    {

                        e.Graphics.DrawString(Member.Param[i].Name + ":", paramFont, text_color, rect_param);
                        e.Graphics.DrawString(Member.Param[i].Desc, this.font, text_color, rect_param.X + e.Graphics.MeasureString(Member.Param[i].Name + ":", paramFont, new Size()).ToSize().Width, rect_param.Y);
                        rect_param.Y += e.Graphics.MeasureString(Member.Param[i].Desc, this.Font, new Size()).ToSize().Height;
                    }
                }
                //foreach (var item in Params)
                //{


                //}
                text_color.Dispose();
                text_datatype.Dispose();
                text_string.Dispose();
            }
            else
            {

                Size text_size = e.Graphics.MeasureString(e.ToolTipText, this.Font, new Size()).ToSize();
                SolidBrush text_sb = new SolidBrush(this.ForeColor);
                e.Graphics.DrawString(e.ToolTipText, this.Font, text_sb, text_rect);

                text_sb.Dispose();
            }

            #endregion

        }

        /// <summary>
        /// 通过文本计算工具提示大小(text为null时根据control的文本计算)
        /// </summary>
        /// <param name="control">要为其检索 System.Windows.Forms.ToolTip 文本的 System.Windows.Forms.Control。</param>
        /// <param name="text">要计算的文本</param>
        /// <returns></returns>
        private Size GetToolTipSize(Control control, string text = null)
        {
            Graphics g = control.CreateGraphics();
            string text_str = text == null ? this.GetToolTip(control) : text;
            Size text_size = TextRenderer.MeasureText(g, text_str, Font,new Size(),TextFormatFlags.ExternalLeading| TextFormatFlags.NoPadding| TextFormatFlags.NoPrefix);
            //Size text_size = g.MeasureString(text_str, this.Font, new Size()).ToSize();
            text_size.Width += this.Padding * 2;
            text_size.Height += this.Padding * 2;

            if (this.MinSize.Width > 0 && this.MinSize.Width > text_size.Width)
                text_size.Width = this.MinSize.Width;
            if (this.MinSize.Height > 0 && this.MinSize.Height > text_size.Height)
                text_size.Height = this.MinSize.Height;

            if (this.MaxSize.Width > 0 && text_size.Width > this.MaxSize.Width)
                text_size.Width = this.MaxSize.Width;
            if (this.MaxSize.Height > 0 && text_size.Height > this.MaxSize.Height)
                text_size.Height = this.MaxSize.Height;


            g.Dispose();
            return text_size;
        }

        #endregion

        #region 枚举

        /// <summary>
        /// 提示框标题位置
        /// </summary>
        [Description("提示框标题位置")]
        public enum TitleAnchor
        {
            /// <summary>
            /// 顶部
            /// </summary>
            Top,
            /// <summary>
            /// 左边
            /// </summary>
            Left,
            /// <summary>
            /// 右边
            /// </summary>
            Right,
            /// <summary>
            /// 下边
            /// </summary>
            Bottom
        }

        /// <summary>
        /// 提示框位置
        /// </summary>
        [Description("提示框位置")]
        public enum ToolTipAnchor
        {
            /// <summary>
            /// 顶部居中
            /// </summary>
            TopCenter,
            /// <summary>
            /// 左边居中
            /// </summary>
            LeftCenter,
            /// <summary>
            /// 右边居中
            /// </summary>
            RightCenter,
            /// <summary>
            /// 下边居中
            /// </summary>
            BottomCenter
        }

        #endregion
    }

}

namespace RSCorrectTest
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnEncode = new System.Windows.Forms.Button();
            this.txtInData = new System.Windows.Forms.TextBox();
            this.lblInData = new System.Windows.Forms.Label();
            this.lblCorrect = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.txtCorrData = new System.Windows.Forms.TextBox();
            this.txtErrData = new System.Windows.Forms.TextBox();
            this.txtErrParity = new System.Windows.Forms.TextBox();
            this.btnDecode = new System.Windows.Forms.Button();
            this.txtCorrParity = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnEncode
            // 
            this.btnEncode.Location = new System.Drawing.Point(38, 61);
            this.btnEncode.Name = "btnEncode";
            this.btnEncode.Size = new System.Drawing.Size(75, 23);
            this.btnEncode.TabIndex = 0;
            this.btnEncode.Text = "エンコード";
            this.btnEncode.UseVisualStyleBackColor = true;
            this.btnEncode.Click += new System.EventHandler(this.BtnEncode_Click);
            // 
            // txtInData
            // 
            this.txtInData.Location = new System.Drawing.Point(154, 31);
            this.txtInData.Name = "txtInData";
            this.txtInData.Size = new System.Drawing.Size(243, 19);
            this.txtInData.TabIndex = 1;
            this.txtInData.Text = "The quick brown fox jumps over the lazy dog";
            // 
            // lblInData
            // 
            this.lblInData.AutoSize = true;
            this.lblInData.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblInData.Location = new System.Drawing.Point(35, 31);
            this.lblInData.Name = "lblInData";
            this.lblInData.Size = new System.Drawing.Size(68, 18);
            this.lblInData.TabIndex = 2;
            this.lblInData.Text = "入力データ";
            // 
            // lblCorrect
            // 
            this.lblCorrect.AutoSize = true;
            this.lblCorrect.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblCorrect.Location = new System.Drawing.Point(35, 181);
            this.lblCorrect.Name = "lblCorrect";
            this.lblCorrect.Size = new System.Drawing.Size(68, 18);
            this.lblCorrect.TabIndex = 3;
            this.lblCorrect.Text = "訂正データ";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblError.Location = new System.Drawing.Point(35, 106);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(92, 18);
            this.lblError.TabIndex = 4;
            this.lblError.Text = "誤り付加データ";
            // 
            // txtCorrData
            // 
            this.txtCorrData.Location = new System.Drawing.Point(154, 181);
            this.txtCorrData.Name = "txtCorrData";
            this.txtCorrData.Size = new System.Drawing.Size(243, 19);
            this.txtCorrData.TabIndex = 5;
            // 
            // txtErrData
            // 
            this.txtErrData.Location = new System.Drawing.Point(154, 106);
            this.txtErrData.Name = "txtErrData";
            this.txtErrData.Size = new System.Drawing.Size(243, 19);
            this.txtErrData.TabIndex = 6;
            // 
            // txtErrParity
            // 
            this.txtErrParity.Location = new System.Drawing.Point(409, 106);
            this.txtErrParity.Name = "txtErrParity";
            this.txtErrParity.ReadOnly = true;
            this.txtErrParity.Size = new System.Drawing.Size(221, 19);
            this.txtErrParity.TabIndex = 7;
            // 
            // btnDecode
            // 
            this.btnDecode.Location = new System.Drawing.Point(38, 136);
            this.btnDecode.Name = "btnDecode";
            this.btnDecode.Size = new System.Drawing.Size(75, 23);
            this.btnDecode.TabIndex = 8;
            this.btnDecode.Text = "デコード";
            this.btnDecode.UseVisualStyleBackColor = true;
            this.btnDecode.Click += new System.EventHandler(this.BtnDecode_Click);
            // 
            // txtCorrParity
            // 
            this.txtCorrParity.Location = new System.Drawing.Point(409, 181);
            this.txtCorrParity.Name = "txtCorrParity";
            this.txtCorrParity.ReadOnly = true;
            this.txtCorrParity.Size = new System.Drawing.Size(221, 19);
            this.txtCorrParity.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 232);
            this.Controls.Add(this.txtCorrParity);
            this.Controls.Add(this.btnDecode);
            this.Controls.Add(this.txtErrParity);
            this.Controls.Add(this.txtErrData);
            this.Controls.Add(this.txtCorrData);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.lblCorrect);
            this.Controls.Add(this.lblInData);
            this.Controls.Add(this.txtInData);
            this.Controls.Add(this.btnEncode);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEncode;
        private System.Windows.Forms.TextBox txtInData;
        private System.Windows.Forms.Label lblInData;
        private System.Windows.Forms.Label lblCorrect;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.TextBox txtCorrData;
        private System.Windows.Forms.TextBox txtErrData;
        private System.Windows.Forms.TextBox txtErrParity;
        private System.Windows.Forms.Button btnDecode;
        private System.Windows.Forms.TextBox txtCorrParity;
    }
}


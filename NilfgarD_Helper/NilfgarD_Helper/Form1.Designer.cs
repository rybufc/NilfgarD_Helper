namespace NilfgarD_Helper
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkNicknameButton = new System.Windows.Forms.Button();
            this.nicknameBox = new System.Windows.Forms.TextBox();
            this.getListOfBannedMembersButton = new System.Windows.Forms.Button();
            this.ClearRequestThreadButton = new System.Windows.Forms.Button();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.loginTextBox = new System.Windows.Forms.TextBox();
            this.vkAuthButton = new System.Windows.Forms.Button();
            this.informationTextBox = new System.Windows.Forms.TextBox();
            this.deleteBox = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.deleteMessageButton = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button8 = new System.Windows.Forms.Button();
            this.openRequestFileButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkNicknameButton
            // 
            this.checkNicknameButton.Location = new System.Drawing.Point(197, 90);
            this.checkNicknameButton.Name = "checkNicknameButton";
            this.checkNicknameButton.Size = new System.Drawing.Size(75, 23);
            this.checkNicknameButton.TabIndex = 6;
            this.checkNicknameButton.Text = "Проверить";
            this.checkNicknameButton.UseVisualStyleBackColor = true;
            this.checkNicknameButton.Click += new System.EventHandler(this.checkNicknameButton_Click);
            // 
            // nicknameBox
            // 
            this.nicknameBox.Location = new System.Drawing.Point(12, 93);
            this.nicknameBox.Name = "nicknameBox";
            this.nicknameBox.Size = new System.Drawing.Size(179, 20);
            this.nicknameBox.TabIndex = 5;
            this.nicknameBox.Text = "Введите ник на проверку...";
            // 
            // getListOfBannedMembersButton
            // 
            this.getListOfBannedMembersButton.Location = new System.Drawing.Point(12, 64);
            this.getListOfBannedMembersButton.Name = "getListOfBannedMembersButton";
            this.getListOfBannedMembersButton.Size = new System.Drawing.Size(260, 23);
            this.getListOfBannedMembersButton.TabIndex = 4;
            this.getListOfBannedMembersButton.Text = "Получить список забаненных пользователей...";
            this.getListOfBannedMembersButton.UseVisualStyleBackColor = false;
            this.getListOfBannedMembersButton.Click += new System.EventHandler(this.getListOfBannedMembersButton_Click);
            // 
            // ClearRequestThreadButton
            // 
            this.ClearRequestThreadButton.Location = new System.Drawing.Point(12, 145);
            this.ClearRequestThreadButton.Name = "ClearRequestThreadButton";
            this.ClearRequestThreadButton.Size = new System.Drawing.Size(260, 23);
            this.ClearRequestThreadButton.TabIndex = 7;
            this.ClearRequestThreadButton.Text = "Подчистить тему заявок...";
            this.ClearRequestThreadButton.UseVisualStyleBackColor = true;
            this.ClearRequestThreadButton.Click += new System.EventHandler(this.ClearRequestThreadButton_Click);
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(12, 38);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(157, 20);
            this.passwordTextBox.TabIndex = 2;
            this.passwordTextBox.Text = "Password";
            // 
            // loginTextBox
            // 
            this.loginTextBox.Location = new System.Drawing.Point(12, 12);
            this.loginTextBox.Name = "loginTextBox";
            this.loginTextBox.Size = new System.Drawing.Size(157, 20);
            this.loginTextBox.TabIndex = 1;
            this.loginTextBox.Text = "Login";
            // 
            // vkAuthButton
            // 
            this.vkAuthButton.Location = new System.Drawing.Point(175, 10);
            this.vkAuthButton.Name = "vkAuthButton";
            this.vkAuthButton.Size = new System.Drawing.Size(97, 48);
            this.vkAuthButton.TabIndex = 3;
            this.vkAuthButton.Text = "Войти";
            this.vkAuthButton.UseVisualStyleBackColor = true;
            this.vkAuthButton.Click += new System.EventHandler(this.vkAuthButton_Click);
            // 
            // informationTextBox
            // 
            this.informationTextBox.Location = new System.Drawing.Point(278, 12);
            this.informationTextBox.Multiline = true;
            this.informationTextBox.Name = "informationTextBox";
            this.informationTextBox.ReadOnly = true;
            this.informationTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.informationTextBox.Size = new System.Drawing.Size(287, 298);
            this.informationTextBox.TabIndex = 8;
            this.informationTextBox.Text = "Информация...";
            // 
            // deleteBox
            // 
            this.deleteBox.Location = new System.Drawing.Point(12, 203);
            this.deleteBox.Name = "deleteBox";
            this.deleteBox.Size = new System.Drawing.Size(157, 20);
            this.deleteBox.TabIndex = 9;
            this.deleteBox.Text = "Введите ник для удаления...";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(12, 261);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(260, 20);
            this.textBox2.TabIndex = 10;
            // 
            // deleteMessageButton
            // 
            this.deleteMessageButton.Location = new System.Drawing.Point(175, 203);
            this.deleteMessageButton.Name = "deleteMessageButton";
            this.deleteMessageButton.Size = new System.Drawing.Size(97, 23);
            this.deleteMessageButton.TabIndex = 11;
            this.deleteMessageButton.Text = "Удалить заявку";
            this.deleteMessageButton.UseVisualStyleBackColor = true;
            this.deleteMessageButton.Click += new System.EventHandler(this.deleteMessageButton_Click);
            // 
            // button6
            // 
            this.button6.Enabled = false;
            this.button6.Location = new System.Drawing.Point(12, 287);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(113, 23);
            this.button6.TabIndex = 12;
            this.button6.Text = "Принял";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Enabled = false;
            this.button7.Location = new System.Drawing.Point(159, 287);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(113, 23);
            this.button7.TabIndex = 13;
            this.button7.Text = "Не принял";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(100, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Admin-bar";
            // 
            // button8
            // 
            this.button8.Enabled = false;
            this.button8.Location = new System.Drawing.Point(12, 232);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(260, 23);
            this.button8.TabIndex = 15;
            this.button8.Text = "Начать приём в клуб...";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // openRequestFileButton
            // 
            this.openRequestFileButton.Location = new System.Drawing.Point(12, 174);
            this.openRequestFileButton.Name = "openRequestFileButton";
            this.openRequestFileButton.Size = new System.Drawing.Size(260, 23);
            this.openRequestFileButton.TabIndex = 16;
            this.openRequestFileButton.Text = "Открыть файл с заявками...";
            this.openRequestFileButton.UseVisualStyleBackColor = true;
            this.openRequestFileButton.Click += new System.EventHandler(this.openRequestFileButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 320);
            this.Controls.Add(this.openRequestFileButton);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.deleteMessageButton);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.deleteBox);
            this.Controls.Add(this.informationTextBox);
            this.Controls.Add(this.vkAuthButton);
            this.Controls.Add(this.loginTextBox);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.ClearRequestThreadButton);
            this.Controls.Add(this.getListOfBannedMembersButton);
            this.Controls.Add(this.nicknameBox);
            this.Controls.Add(this.checkNicknameButton);
            this.MaximumSize = new System.Drawing.Size(593, 500);
            this.MinimumSize = new System.Drawing.Size(593, 39);
            this.Name = "Form1";
            this.Text = "NilfgarD Helper";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button checkNicknameButton;
        private System.Windows.Forms.TextBox nicknameBox;
        private System.Windows.Forms.Button getListOfBannedMembersButton;
        private System.Windows.Forms.Button ClearRequestThreadButton;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox loginTextBox;
        private System.Windows.Forms.Button vkAuthButton;
        private System.Windows.Forms.TextBox informationTextBox;
        private System.Windows.Forms.TextBox deleteBox;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button deleteMessageButton;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button openRequestFileButton;
    }
}


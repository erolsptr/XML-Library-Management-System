namespace Library.Client.WinForms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            lblAuthStatus = new Label();
            btnLogin = new Button();
            txtPassword = new TextBox();
            label2 = new Label();
            txtUsername = new TextBox();
            label1 = new Label();
            groupBox2 = new GroupBox();
            btnGetAllBooks = new Button();
            btnHtmlReport = new Button();
            btnDeleteBook = new Button();
            btnGetBookById = new Button();
            txtBookId = new TextBox();
            dgvBooks = new DataGridView();
            groupBox3 = new GroupBox();
            btnFetchBookDetails = new Button();
            btnUpdateBook = new Button();
            btnAddBook = new Button();
            txtGenre = new TextBox();
            label3 = new Label();
            txtYear = new TextBox();
            label7 = new Label();
            txtIsbn = new TextBox();
            label6 = new Label();
            txtAuthor = new TextBox();
            label4 = new Label();
            txtTitle = new TextBox();
            label5 = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBooks).BeginInit();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lblAuthStatus);
            groupBox1.Controls.Add(btnLogin);
            groupBox1.Controls.Add(txtPassword);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(txtUsername);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 345);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(195, 204);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Authentication";
            // 
            // lblAuthStatus
            // 
            lblAuthStatus.AutoSize = true;
            lblAuthStatus.ForeColor = Color.Red;
            lblAuthStatus.Location = new Point(39, 177);
            lblAuthStatus.Name = "lblAuthStatus";
            lblAuthStatus.Size = new Size(121, 15);
            lblAuthStatus.TabIndex = 5;
            lblAuthStatus.Text = "Status: Not Logged In";
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(59, 122);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(75, 23);
            btnLogin.TabIndex = 4;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(84, 58);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(100, 23);
            txtPassword.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 61);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 2;
            label2.Text = "Password:";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(84, 32);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(100, 23);
            txtUsername.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 35);
            label1.Name = "label1";
            label1.Size = new Size(63, 15);
            label1.TabIndex = 0;
            label1.Text = "Username:";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnGetAllBooks);
            groupBox2.Controls.Add(btnHtmlReport);
            groupBox2.Controls.Add(btnDeleteBook);
            groupBox2.Controls.Add(btnGetBookById);
            groupBox2.Controls.Add(txtBookId);
            groupBox2.Location = new Point(677, 345);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(295, 215);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Book Operations";
            // 
            // btnGetAllBooks
            // 
            btnGetAllBooks.Location = new Point(17, 35);
            btnGetAllBooks.Name = "btnGetAllBooks";
            btnGetAllBooks.Size = new Size(136, 23);
            btnGetAllBooks.TabIndex = 8;
            btnGetAllBooks.Text = "Get All Books";
            btnGetAllBooks.UseVisualStyleBackColor = true;
            btnGetAllBooks.Click += btnGetAllBooks_Click;
            // 
            // btnHtmlReport
            // 
            btnHtmlReport.Location = new Point(17, 177);
            btnHtmlReport.Name = "btnHtmlReport";
            btnHtmlReport.Size = new Size(136, 23);
            btnHtmlReport.TabIndex = 7;
            btnHtmlReport.Text = "Generate HTML Report";
            btnHtmlReport.UseVisualStyleBackColor = true;
            btnHtmlReport.Click += btnHtmlReport_Click;
            // 
            // btnDeleteBook
            // 
            btnDeleteBook.Location = new Point(17, 142);
            btnDeleteBook.Name = "btnDeleteBook";
            btnDeleteBook.Size = new Size(136, 23);
            btnDeleteBook.TabIndex = 6;
            btnDeleteBook.Text = "Delete Book by ID";
            btnDeleteBook.UseVisualStyleBackColor = true;
            btnDeleteBook.Click += btnDeleteBook_Click;
            // 
            // btnGetBookById
            // 
            btnGetBookById.Location = new Point(17, 103);
            btnGetBookById.Name = "btnGetBookById";
            btnGetBookById.Size = new Size(136, 23);
            btnGetBookById.TabIndex = 5;
            btnGetBookById.Text = "Get Book by ID";
            btnGetBookById.UseVisualStyleBackColor = true;
            btnGetBookById.Click += btnGetBookById_Click;
            // 
            // txtBookId
            // 
            txtBookId.Location = new Point(180, 142);
            txtBookId.Name = "txtBookId";
            txtBookId.Size = new Size(100, 23);
            txtBookId.TabIndex = 3;
            // 
            // dgvBooks
            // 
            dgvBooks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvBooks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBooks.Location = new Point(43, 6);
            dgvBooks.Name = "dgvBooks";
            dgvBooks.ReadOnly = true;
            dgvBooks.Size = new Size(885, 333);
            dgvBooks.TabIndex = 2;
            dgvBooks.SelectionChanged += dgvBooks_SelectionChanged;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnFetchBookDetails);
            groupBox3.Controls.Add(btnUpdateBook);
            groupBox3.Controls.Add(btnAddBook);
            groupBox3.Controls.Add(txtGenre);
            groupBox3.Controls.Add(label3);
            groupBox3.Controls.Add(txtYear);
            groupBox3.Controls.Add(label7);
            groupBox3.Controls.Add(txtIsbn);
            groupBox3.Controls.Add(label6);
            groupBox3.Controls.Add(txtAuthor);
            groupBox3.Controls.Add(label4);
            groupBox3.Controls.Add(txtTitle);
            groupBox3.Controls.Add(label5);
            groupBox3.Location = new Point(290, 345);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(340, 204);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Text = "Add / Update Book";
            // 
            // btnFetchBookDetails
            // 
            btnFetchBookDetails.Location = new Point(205, 85);
            btnFetchBookDetails.Name = "btnFetchBookDetails";
            btnFetchBookDetails.Size = new Size(135, 23);
            btnFetchBookDetails.TabIndex = 14;
            btnFetchBookDetails.Text = "Fetch Details by ISBN";
            btnFetchBookDetails.UseVisualStyleBackColor = true;
            btnFetchBookDetails.Click += btnFetchBookDetails_Click;
            // 
            // btnUpdateBook
            // 
            btnUpdateBook.Location = new Point(157, 175);
            btnUpdateBook.Name = "btnUpdateBook";
            btnUpdateBook.Size = new Size(177, 23);
            btnUpdateBook.TabIndex = 13;
            btnUpdateBook.Text = "Update Book (by ID in list)";
            btnUpdateBook.UseVisualStyleBackColor = true;
            btnUpdateBook.Click += btnUpdateBook_Click;
            // 
            // btnAddBook
            // 
            btnAddBook.Location = new Point(6, 175);
            btnAddBook.Name = "btnAddBook";
            btnAddBook.Size = new Size(136, 23);
            btnAddBook.TabIndex = 12;
            btnAddBook.Text = "Add New Book";
            btnAddBook.UseVisualStyleBackColor = true;
            btnAddBook.Click += btnAddBook_Click;
            // 
            // txtGenre
            // 
            txtGenre.Location = new Point(84, 143);
            txtGenre.Name = "txtGenre";
            txtGenre.Size = new Size(100, 23);
            txtGenre.TabIndex = 11;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(21, 152);
            label3.Name = "label3";
            label3.Size = new Size(41, 15);
            label3.TabIndex = 10;
            label3.Text = "Genre:";
            // 
            // txtYear
            // 
            txtYear.Location = new Point(84, 114);
            txtYear.Name = "txtYear";
            txtYear.Size = new Size(100, 23);
            txtYear.TabIndex = 9;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(18, 122);
            label7.Name = "label7";
            label7.Size = new Size(32, 15);
            label7.TabIndex = 8;
            label7.Text = "Year:";
            // 
            // txtIsbn
            // 
            txtIsbn.Location = new Point(84, 85);
            txtIsbn.Name = "txtIsbn";
            txtIsbn.Size = new Size(100, 23);
            txtIsbn.TabIndex = 7;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(18, 85);
            label6.Name = "label6";
            label6.Size = new Size(35, 15);
            label6.TabIndex = 6;
            label6.Text = "ISBN:";
            // 
            // txtAuthor
            // 
            txtAuthor.Location = new Point(84, 58);
            txtAuthor.Name = "txtAuthor";
            txtAuthor.Size = new Size(100, 23);
            txtAuthor.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(18, 61);
            label4.Name = "label4";
            label4.Size = new Size(47, 15);
            label4.TabIndex = 2;
            label4.Text = "Author:";
            // 
            // txtTitle
            // 
            txtTitle.Location = new Point(84, 32);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(100, 23);
            txtTitle.TabIndex = 1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(15, 35);
            label5.Name = "label5";
            label5.Size = new Size(33, 15);
            label5.TabIndex = 0;
            label5.Text = "Title:";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 561);
            Controls.Add(groupBox3);
            Controls.Add(dgvBooks);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Library Management Client";
            Load += MainForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBooks).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private TextBox txtUsername;
        private Label label1;
        private Label lblAuthStatus;
        private Button btnLogin;
        private TextBox txtPassword;
        private Label label2;
        private GroupBox groupBox2;
        private Label label3;
        private Button button1;
        private TextBox txtBookId;
        private Button btnGetBookById;
        private Button btnHtmlReport;
        private Button btnDeleteBook;
        private DataGridView dgvBooks;
        private GroupBox groupBox3;
        private TextBox txtGenre;
        private TextBox txtYear;
        private Label label7;
        private TextBox txtIsbn;
        private Label label6;
        private TextBox txtAuthor;
        private TextBox txtTitle;
        private Label label5;
        private Label label4;
        private Button btnGetAllBooks;
        private Button btnUpdateBook;
        private Button btnAddBook;
        private Button btnFetchBookDetails;
    }
}

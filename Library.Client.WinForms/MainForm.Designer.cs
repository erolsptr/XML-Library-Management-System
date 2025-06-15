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
            groupBox2 = new GroupBox();
            btnSearchBooks = new Button();
            txtSearchTerm = new TextBox();
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
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            btnLoanBook = new Button();
            txtLoanMemberId = new TextBox();
            label8 = new Label();
            groupBox1 = new GroupBox();
            lblAuthStatus = new Label();
            btnLogin = new Button();
            txtPassword = new TextBox();
            label2 = new Label();
            txtUsername = new TextBox();
            label1 = new Label();
            tabPage2 = new TabPage();
            groupBox5 = new GroupBox();
            lstMemberLoans = new ListBox();
            groupBox4 = new GroupBox();
            dtpMembershipDate = new DateTimePicker();
            label9 = new Label();
            txtMemberFirstName = new TextBox();
            label11 = new Label();
            label10 = new Label();
            btnAddMember = new Button();
            txtMemberLastName = new TextBox();
            txtMemberEmail = new TextBox();
            btnDeleteMember = new Button();
            btnGetAllMembers = new Button();
            dgvMembers = new DataGridView();
            tabPage3 = new TabPage();
            btnReturnBook = new Button();
            dgvLoans = new DataGridView();
            btnGetAllLoans = new Button();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBooks).BeginInit();
            groupBox3.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox1.SuspendLayout();
            tabPage2.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMembers).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLoans).BeginInit();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnSearchBooks);
            groupBox2.Controls.Add(txtSearchTerm);
            groupBox2.Controls.Add(btnGetAllBooks);
            groupBox2.Controls.Add(btnHtmlReport);
            groupBox2.Controls.Add(btnDeleteBook);
            groupBox2.Controls.Add(btnGetBookById);
            groupBox2.Controls.Add(txtBookId);
            groupBox2.Location = new Point(282, 6);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(315, 215);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Book Operations";
            // 
            // btnSearchBooks
            // 
            btnSearchBooks.Location = new Point(134, 77);
            btnSearchBooks.Name = "btnSearchBooks";
            btnSearchBooks.Size = new Size(136, 23);
            btnSearchBooks.TabIndex = 10;
            btnSearchBooks.Text = "Search by Title";
            btnSearchBooks.UseVisualStyleBackColor = true;
            btnSearchBooks.Click += btnSearchBooks_Click;
            // 
            // txtSearchTerm
            // 
            txtSearchTerm.Location = new Point(6, 81);
            txtSearchTerm.Name = "txtSearchTerm";
            txtSearchTerm.Size = new Size(100, 23);
            txtSearchTerm.TabIndex = 9;
            // 
            // btnGetAllBooks
            // 
            btnGetAllBooks.Location = new Point(77, 22);
            btnGetAllBooks.Name = "btnGetAllBooks";
            btnGetAllBooks.Size = new Size(136, 23);
            btnGetAllBooks.TabIndex = 8;
            btnGetAllBooks.Text = "Get All Books";
            btnGetAllBooks.UseVisualStyleBackColor = true;
            btnGetAllBooks.Click += btnGetAllBooks_Click;
            // 
            // btnHtmlReport
            // 
            btnHtmlReport.Location = new Point(179, 186);
            btnHtmlReport.Name = "btnHtmlReport";
            btnHtmlReport.Size = new Size(136, 23);
            btnHtmlReport.TabIndex = 7;
            btnHtmlReport.Text = "Generate HTML Report";
            btnHtmlReport.UseVisualStyleBackColor = true;
            btnHtmlReport.Click += btnHtmlReport_Click;
            // 
            // btnDeleteBook
            // 
            btnDeleteBook.Location = new Point(134, 144);
            btnDeleteBook.Name = "btnDeleteBook";
            btnDeleteBook.Size = new Size(136, 23);
            btnDeleteBook.TabIndex = 6;
            btnDeleteBook.Text = "Delete Book by ID";
            btnDeleteBook.UseVisualStyleBackColor = true;
            btnDeleteBook.Click += btnDeleteBook_Click;
            // 
            // btnGetBookById
            // 
            btnGetBookById.Location = new Point(134, 122);
            btnGetBookById.Name = "btnGetBookById";
            btnGetBookById.Size = new Size(136, 23);
            btnGetBookById.TabIndex = 5;
            btnGetBookById.Text = "Get Book by ID";
            btnGetBookById.UseVisualStyleBackColor = true;
            btnGetBookById.Click += btnGetBookById_Click;
            // 
            // txtBookId
            // 
            txtBookId.Location = new Point(6, 132);
            txtBookId.Name = "txtBookId";
            txtBookId.Size = new Size(100, 23);
            txtBookId.TabIndex = 3;
            // 
            // dgvBooks
            // 
            dgvBooks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBooks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBooks.Location = new Point(33, 241);
            dgvBooks.Name = "dgvBooks";
            dgvBooks.ReadOnly = true;
            dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBooks.Size = new Size(754, 377);
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
            groupBox3.Location = new Point(603, 6);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(340, 215);
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
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(-3, -2);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1011, 652);
            tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnLoanBook);
            tabPage1.Controls.Add(txtLoanMemberId);
            tabPage1.Controls.Add(label8);
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Controls.Add(groupBox3);
            tabPage1.Controls.Add(dgvBooks);
            tabPage1.Controls.Add(groupBox2);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1003, 624);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Books";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnLoanBook
            // 
            btnLoanBook.Location = new Point(793, 284);
            btnLoanBook.Name = "btnLoanBook";
            btnLoanBook.Size = new Size(122, 23);
            btnLoanBook.TabIndex = 7;
            btnLoanBook.Text = "Loan Selected Book";
            btnLoanBook.UseVisualStyleBackColor = true;
            btnLoanBook.Click += btnLoanBook_Click;
            // 
            // txtLoanMemberId
            // 
            txtLoanMemberId.Location = new Point(911, 255);
            txtLoanMemberId.Name = "txtLoanMemberId";
            txtLoanMemberId.Size = new Size(45, 23);
            txtLoanMemberId.TabIndex = 6;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(793, 258);
            label8.Name = "label8";
            label8.Size = new Size(112, 15);
            label8.TabIndex = 5;
            label8.Text = "Member ID to Loan:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lblAuthStatus);
            groupBox1.Controls.Add(btnLogin);
            groupBox1.Controls.Add(txtPassword);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(txtUsername);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(33, 17);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(195, 204);
            groupBox1.TabIndex = 4;
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
            btnLogin.Location = new Point(58, 141);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(75, 23);
            btnLogin.TabIndex = 4;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(85, 78);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(100, 23);
            txtPassword.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(19, 81);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 2;
            label2.Text = "Password:";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(85, 52);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(100, 23);
            txtUsername.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 55);
            label1.Name = "label1";
            label1.Size = new Size(63, 15);
            label1.TabIndex = 0;
            label1.Text = "Username:";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(groupBox5);
            tabPage2.Controls.Add(groupBox4);
            tabPage2.Controls.Add(btnDeleteMember);
            tabPage2.Controls.Add(btnGetAllMembers);
            tabPage2.Controls.Add(dgvMembers);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1003, 624);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Members";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(lstMemberLoans);
            groupBox5.Location = new Point(41, 352);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(210, 217);
            groupBox5.TabIndex = 13;
            groupBox5.TabStop = false;
            groupBox5.Text = "Selected Member's Active Loans";
            // 
            // lstMemberLoans
            // 
            lstMemberLoans.FormattingEnabled = true;
            lstMemberLoans.ItemHeight = 15;
            lstMemberLoans.Location = new Point(6, 22);
            lstMemberLoans.Name = "lstMemberLoans";
            lstMemberLoans.Size = new Size(198, 184);
            lstMemberLoans.TabIndex = 14;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(dtpMembershipDate);
            groupBox4.Controls.Add(label9);
            groupBox4.Controls.Add(txtMemberFirstName);
            groupBox4.Controls.Add(label11);
            groupBox4.Controls.Add(label10);
            groupBox4.Controls.Add(btnAddMember);
            groupBox4.Controls.Add(txtMemberLastName);
            groupBox4.Controls.Add(txtMemberEmail);
            groupBox4.Location = new Point(41, 16);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(210, 236);
            groupBox4.TabIndex = 12;
            groupBox4.TabStop = false;
            groupBox4.Text = "New Member";
            // 
            // dtpMembershipDate
            // 
            dtpMembershipDate.Format = DateTimePickerFormat.Short;
            dtpMembershipDate.Location = new Point(4, 167);
            dtpMembershipDate.Name = "dtpMembershipDate";
            dtpMembershipDate.Size = new Size(200, 23);
            dtpMembershipDate.TabIndex = 13;
            dtpMembershipDate.Visible = false;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(6, 37);
            label9.Name = "label9";
            label9.Size = new Size(67, 15);
            label9.TabIndex = 8;
            label9.Text = "First Name:";
            // 
            // txtMemberFirstName
            // 
            txtMemberFirstName.Location = new Point(79, 34);
            txtMemberFirstName.Name = "txtMemberFirstName";
            txtMemberFirstName.Size = new Size(100, 23);
            txtMemberFirstName.TabIndex = 2;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(6, 128);
            label11.Name = "label11";
            label11.Size = new Size(39, 15);
            label11.TabIndex = 10;
            label11.Text = "Email:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(6, 86);
            label10.Name = "label10";
            label10.Size = new Size(66, 15);
            label10.TabIndex = 9;
            label10.Text = "Last Name:";
            // 
            // btnAddMember
            // 
            btnAddMember.Location = new Point(49, 207);
            btnAddMember.Name = "btnAddMember";
            btnAddMember.Size = new Size(100, 23);
            btnAddMember.TabIndex = 5;
            btnAddMember.Text = "Add Member";
            btnAddMember.UseVisualStyleBackColor = true;
            btnAddMember.Click += btnAddMember_Click;
            // 
            // txtMemberLastName
            // 
            txtMemberLastName.Location = new Point(79, 83);
            txtMemberLastName.Name = "txtMemberLastName";
            txtMemberLastName.Size = new Size(100, 23);
            txtMemberLastName.TabIndex = 3;
            // 
            // txtMemberEmail
            // 
            txtMemberEmail.Location = new Point(79, 125);
            txtMemberEmail.Name = "txtMemberEmail";
            txtMemberEmail.Size = new Size(100, 23);
            txtMemberEmail.TabIndex = 4;
            // 
            // btnDeleteMember
            // 
            btnDeleteMember.Location = new Point(164, 258);
            btnDeleteMember.Name = "btnDeleteMember";
            btnDeleteMember.Size = new Size(100, 23);
            btnDeleteMember.TabIndex = 7;
            btnDeleteMember.Text = "Delete Member";
            btnDeleteMember.UseVisualStyleBackColor = true;
            btnDeleteMember.Click += btnDeleteMember_Click;
            // 
            // btnGetAllMembers
            // 
            btnGetAllMembers.Location = new Point(41, 258);
            btnGetAllMembers.Name = "btnGetAllMembers";
            btnGetAllMembers.Size = new Size(117, 23);
            btnGetAllMembers.TabIndex = 1;
            btnGetAllMembers.Text = "Get All Members";
            btnGetAllMembers.UseVisualStyleBackColor = true;
            btnGetAllMembers.Click += btnGetAllMembers_Click;
            // 
            // dgvMembers
            // 
            dgvMembers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvMembers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMembers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMembers.Location = new Point(275, 6);
            dgvMembers.Name = "dgvMembers";
            dgvMembers.ReadOnly = true;
            dgvMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMembers.Size = new Size(688, 598);
            dgvMembers.TabIndex = 0;
            dgvMembers.SelectionChanged += dgvMembers_SelectionChanged;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(btnReturnBook);
            tabPage3.Controls.Add(dgvLoans);
            tabPage3.Controls.Add(btnGetAllLoans);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1003, 624);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Loans";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnReturnBook
            // 
            btnReturnBook.Location = new Point(11, 63);
            btnReturnBook.Name = "btnReturnBook";
            btnReturnBook.Size = new Size(136, 23);
            btnReturnBook.TabIndex = 2;
            btnReturnBook.Text = "Return Selected Loan";
            btnReturnBook.UseVisualStyleBackColor = true;
            btnReturnBook.Click += btnReturnBook_Click;
            // 
            // dgvLoans
            // 
            dgvLoans.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvLoans.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLoans.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLoans.Location = new Point(181, 6);
            dgvLoans.Name = "dgvLoans";
            dgvLoans.ReadOnly = true;
            dgvLoans.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLoans.Size = new Size(782, 611);
            dgvLoans.TabIndex = 1;
            // 
            // btnGetAllLoans
            // 
            btnGetAllLoans.Location = new Point(11, 20);
            btnGetAllLoans.Name = "btnGetAllLoans";
            btnGetAllLoans.Size = new Size(136, 23);
            btnGetAllLoans.TabIndex = 0;
            btnGetAllLoans.Text = "Refresh All Loans";
            btnGetAllLoans.UseVisualStyleBackColor = true;
            btnGetAllLoans.Click += btnGetAllLoans_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(976, 651);
            Controls.Add(tabControl1);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Library Management Client";
            Load += MainForm_Load;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBooks).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabPage2.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMembers).EndInit();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLoans).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private GroupBox groupBox2;
        private Label label3;
        private Button btnGetAllMembers;
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
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private DataGridView dgvMembers;
        private Button btnSearchBooks;
        private TextBox txtSearchTerm;
        private GroupBox groupBox1;
        private Label lblAuthStatus;
        private Button btnLogin;
        private TextBox txtPassword;
        private Label label2;
        private TextBox txtUsername;
        private Label label1;
        private Button btnLoanBook;
        private TextBox txtLoanMemberId;
        private Label label8;
        private Label label11;
        private Label label10;
        private Label label9;
        private Button btnDeleteMember;
        private Button btnAddMember;
        private TextBox txtMemberEmail;
        private TextBox txtMemberLastName;
        private TextBox txtMemberFirstName;
        private GroupBox groupBox4;
        private DateTimePicker dtpMembershipDate;
        private GroupBox groupBox5;
        private ListBox lstMemberLoans;
        private TabPage tabPage3;
        private Button btnGetAllLoans;
        private Button btnReturnBook;
        private DataGridView dgvLoans;
    }
}

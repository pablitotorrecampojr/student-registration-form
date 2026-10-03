using System.Text.RegularExpressions;

namespace student_registration_form
{
    public partial class Form1 : Form
    {
        private static readonly Regex StudentIdPattern = new(@"^[A-Za-z0-9][A-Za-z0-9\-]*$", RegexOptions.Compiled);
        private static readonly Regex NamePattern = new(@"^[A-Za-z][A-Za-z\s.'\-]*$", RegexOptions.Compiled);

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!TryValidateInputs())
            {
                return;
            }

            string studentId = txtStudentId.Text.Trim();
            ListViewItem? existing = FindStudent(studentId);

            if (existing != null)
            {
                DialogResult overwrite = MessageBox.Show(
                    $"A student with ID {studentId} already exists. Do you want to update that record?",
                    "Student already exists",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (overwrite != DialogResult.Yes)
                {
                    return;
                }

                ApplyStudentToItem(existing);
            }
            else
            {
                ListViewItem item = new(studentId);
                item.SubItems.Add(string.Empty);
                item.SubItems.Add(string.Empty);
                item.SubItems.Add(string.Empty);
                item.SubItems.Add(string.Empty);
                item.SubItems.Add(string.Empty);
                ApplyStudentToItem(item);
                lstStudents.Items.Add(item);
            }

            ClearInputs();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstStudents.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    "Select a student from the list before deleting.",
                    "No student selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            ListViewItem selected = lstStudents.SelectedItems[0];
            DialogResult confirm = MessageBox.Show(
                $"Delete student {selected.Text} - {selected.SubItems[1].Text}?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            lstStudents.Items.Remove(selected);
            ClearInputs();
        }

        private void lstStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstStudents.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem item = lstStudents.SelectedItems[0];
            txtStudentId.Text = item.Text;
            txtName.Text = item.SubItems[1].Text;
            cmbProgram.SelectedItem = item.SubItems[2].Text;
            cmbYearLevel.SelectedItem = item.SubItems[3].Text;
            rdoMale.Checked = item.SubItems[4].Text == "Male";
            rdoFemale.Checked = item.SubItems[4].Text == "Female";
            cmbEnrollment.SelectedItem = item.SubItems[5].Text;
            errorProvider.Clear();
        }

        private bool TryValidateInputs()
        {
            errorProvider.Clear();
            bool isValid = true;

            string studentId = txtStudentId.Text.Trim();
            if (string.IsNullOrWhiteSpace(studentId))
            {
                errorProvider.SetError(txtStudentId, "Student ID is required.");
                isValid = false;
            }
            else if (studentId.Length < 3 || !StudentIdPattern.IsMatch(studentId))
            {
                errorProvider.SetError(txtStudentId, "Use at least 3 letters or numbers. Hyphens are allowed.");
                isValid = false;
            }

            string name = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                errorProvider.SetError(txtName, "Name is required.");
                isValid = false;
            }
            else if (name.Length < 2 || !NamePattern.IsMatch(name))
            {
                errorProvider.SetError(txtName, "Enter a valid name using letters, spaces, periods, apostrophes, or hyphens.");
                isValid = false;
            }

            if (cmbProgram.SelectedIndex < 0)
            {
                errorProvider.SetError(cmbProgram, "Select a program.");
                isValid = false;
            }

            if (cmbYearLevel.SelectedIndex < 0)
            {
                errorProvider.SetError(cmbYearLevel, "Select a year level.");
                isValid = false;
            }

            if (!rdoMale.Checked && !rdoFemale.Checked)
            {
                errorProvider.SetError(rdoFemale, "Select a gender.");
                isValid = false;
            }

            if (cmbEnrollment.SelectedIndex < 0)
            {
                errorProvider.SetError(cmbEnrollment, "Select an enrollment status.");
                isValid = false;
            }

            if (!isValid)
            {
                MessageBox.Show(
                    "Please correct the highlighted fields before saving.",
                    "Input validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return isValid;
        }

        private void ApplyStudentToItem(ListViewItem item)
        {
            item.Text = txtStudentId.Text.Trim();
            SetSubItem(item, 1, txtName.Text.Trim());
            SetSubItem(item, 2, cmbProgram.SelectedItem?.ToString() ?? string.Empty);
            SetSubItem(item, 3, cmbYearLevel.SelectedItem?.ToString() ?? string.Empty);
            SetSubItem(item, 4, rdoMale.Checked ? "Male" : "Female");
            SetSubItem(item, 5, cmbEnrollment.SelectedItem?.ToString() ?? string.Empty);
        }

        private static void SetSubItem(ListViewItem item, int index, string value)
        {
            while (item.SubItems.Count <= index)
            {
                item.SubItems.Add(string.Empty);
            }

            item.SubItems[index].Text = value;
        }

        private ListViewItem? FindStudent(string studentId)
        {
            foreach (ListViewItem item in lstStudents.Items)
            {
                if (string.Equals(item.Text, studentId, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }

            return null;
        }

        private void ClearInputs()
        {
            txtStudentId.Clear();
            txtName.Clear();
            cmbProgram.SelectedIndex = -1;
            cmbYearLevel.SelectedIndex = -1;
            rdoMale.Checked = false;
            rdoFemale.Checked = false;
            cmbEnrollment.SelectedIndex = -1;
            lstStudents.SelectedItems.Clear();
            errorProvider.Clear();
            txtStudentId.Focus();
        }
    }
}

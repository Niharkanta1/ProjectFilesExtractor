using Godot;
using System;
using System.IO.Compression;
using File = System.IO.File;

public class MainApplication : Control
{

    private const string PROJECT_SETTINGS_PATH = "\\ProjectSettings\\ProjectSettings.asset";
    private const string TEMPLATE_PROJECT_NAME = "SampleProject";
    private const string DEFAULT_COMPANY_NAME = "DefaultCompany";

    private FileDialog projectDialog, templateDialog;
    private ConfirmationDialog errorDialog;
    private LineEdit projectLineEdit, templateLineEdit, projectNameLineEdit,
                companyNameLineEdit, templateProjectNameLineEdit, defaultCompanyNameLineEdit;
    private Label errorText;
    private TextEdit readMeContent;

    private Vector2 _popUpSize = new Vector2(600, 300);
    private Vector2 _errorPopUpSize = new Vector2(300, 200);

    private string plPath, tlPath, projectName, companyName, defaultCompany, templateProjectName, readMeContentText, readMeExtraContentText;
    private bool isNotDefaultCompany, isNotTemplateProjectName;

    public override void _Ready()
    {
        projectDialog = GetNode<FileDialog>("TextureRect/ProjectLocationDialog");
        templateDialog = GetNode<FileDialog>("TextureRect/TemplateLocationDialog");
        errorDialog = GetNode<ConfirmationDialog>("TextureRect/ErrorDialog");
        projectLineEdit = GetNode<LineEdit>("TextureRect/VBoxContainer/HBoxContainer/LineEdit");
        templateLineEdit = GetNode<LineEdit>("TextureRect/VBoxContainer/HBoxContainer2/LineEdit");
        projectNameLineEdit = GetNode<LineEdit>("TextureRect/VBoxContainer/HBoxContainer3/PN_LineEdit");
        companyNameLineEdit = GetNode<LineEdit>("TextureRect/VBoxContainer/HBoxContainer4/CN_LineEdit");
        defaultCompanyNameLineEdit = GetNode<LineEdit>("TextureRect/VBoxContainer/HBoxContainer6/DCN_LineEdit");
        templateProjectNameLineEdit = GetNode<LineEdit>("TextureRect/VBoxContainer/HBoxContainer7/TPN_LineEdit");
        readMeContent = GetNode<TextEdit>("TextureRect/VBoxContainer/ReadMeText");
        defaultCompanyNameLineEdit.Text = DEFAULT_COMPANY_NAME;
        templateProjectNameLineEdit.Text = TEMPLATE_PROJECT_NAME;
        errorText = GetNode<Label>("TextureRect/ErrorDialog/ErroText");

        defaultCompanyNameLineEdit.Editable = isNotDefaultCompany;
        templateProjectNameLineEdit.Editable = isNotTemplateProjectName;
        defaultCompany = DEFAULT_COMPANY_NAME;
        templateProjectName = TEMPLATE_PROJECT_NAME;
        readMeContentText = "# " + templateProjectName;
    }

    public void _on_PL_Button_pressed()
    {
        projectDialog.PopupCentered(_popUpSize);
    }

    public void _on_TL_Button_pressed()
    {
        templateDialog.PopupCentered(_popUpSize);
    }


    public void _on_ProjectLocationDialog_dir_selected(string dir)
    {
        plPath = dir;
        projectLineEdit.Text = dir;
    }

    public void _on_TemplateLocationDialog_file_selected(string path)
    {
        tlPath = path;
        templateLineEdit.Text = path;
    }

    public void _on_PN_LineEdit_text_changed(string input)
    {
        projectName = input;
        readMeContentText = "# " + projectName;
    }

    public void _on_CN_LineEdit_text_changed(string input)
    {
        companyName = input;
    }

    public void _on_TPN_CheckButton_toggled(bool flag)
    {
        isNotTemplateProjectName = flag;
        templateProjectNameLineEdit.Editable = flag;
    }

    public void _on_DCN_CheckButton_toggled(bool flag)
    {
        isNotDefaultCompany = flag;
        defaultCompanyNameLineEdit.Editable = flag;
    }

    public void _on_DCN_LineEdit_text_changed(string value)
    {
        defaultCompany = isNotDefaultCompany ? value : DEFAULT_COMPANY_NAME;
    }

    public void _on_TPN_LineEdit_text_changed(string value)
    {
        templateProjectName = isNotTemplateProjectName ? value : TEMPLATE_PROJECT_NAME;
    }

    public void _on_ReadMeText_text_changed()
    {
        readMeExtraContentText = readMeContent.Text;
    }

    public void _on_CreateButton_pressed()
    {
        if (tlPath == null && tlPath.Trim().Length == 0)
        {
            errorText.Text = "Template Path is Empty!\nPlease provide the template location";
            errorDialog.PopupCentered(_errorPopUpSize);
        }
        else if (projectName == null || projectName.Trim().Length == 0)
        {
            errorText.Text = "Project Name is Empty!\nPlease Provide a Project Name.";
            errorDialog.PopupCentered(_errorPopUpSize);
        }
        else
        {
            CreateProjectStructure();
        }
    }

    private void CreateProjectStructure()
    {
        GD.Print("Creating Project files...");
        var projectPath = plPath + "\\" + projectName;
        ZipFile.ExtractToDirectory(tlPath, projectPath);
        GD.Print("Project Created");

        UpdateProjectSettingFile(projectPath);
        UpdateReadMeFile(projectPath);

        errorText.Text = "Project Setup Completed.";
        errorDialog.PopupCentered(_errorPopUpSize);
        ClearAllthePaths();
    }

    private void UpdateReadMeFile(string projectPath)
    {
        var text = readMeContentText;
        text += "\n";
        GD.Print("Extra:" + readMeExtraContentText);
        text += readMeExtraContentText;
        File.WriteAllText(projectPath + "\\README.md", text);
    }

    private void UpdateProjectSettingFile(string projectPath)
    {
        var projectSettings = projectPath + PROJECT_SETTINGS_PATH;
        string text = File.ReadAllText(projectSettings);
        text = text.Replace(templateProjectName, projectName);
        if (companyName != null && companyName.Trim().Length != 0)
        {
            text = text.Replace(defaultCompany, companyName);
        }
        File.WriteAllText(projectSettings, text);
    }

    private void ClearAllthePaths()
    {
        plPath = null;
        projectLineEdit.Text = null;
        tlPath = null;
        templateLineEdit.Text = null;
        projectName = null;
        companyName = null;
        companyNameLineEdit.Text = null;
        projectNameLineEdit.Text = null;
        readMeContent.Text = null;
    }
}

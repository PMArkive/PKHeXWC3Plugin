﻿using PKHeX.Core;

namespace WC3Plugin;

public partial class ECBForm : Form
{
    private readonly SAV3 sav;
    private readonly byte[] ecb;

    private static readonly string FileFilter = $"{TranslationStrings.ECardBerry} (*.ecb)|*.ecb|{TranslationStrings.AllFiles} (*.*)|*.*";

    public ECBForm(SAV3 sav)
    {
        this.sav = sav;

        InitializeComponent();

        if (!MysteryDataUtil.IsEmpty(ecb = sav.ExportECB()))
        {
            TitleBox.Text = sav.EBerryName.Trim();
            ECBExportButton.Enabled = true;
        }
    }

    private void ECBImportButton_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        ofd.Filter = FileFilter;
        ofd.Title = string.Format(TranslationStrings.OpenFile, TranslationStrings.ECardBerry);
        ofd.FilterIndex = 1;

        if (ofd.ShowDialog() == DialogResult.OK)
            ImportECB(ofd.FileName);
    }

    private void ECBExportButton_Click(object sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog();
        sfd.Filter = FileFilter;
        sfd.Title = string.Format(TranslationStrings.SaveFile, TranslationStrings.ECardBerry); ;
        sfd.FilterIndex = 1;

        if (sfd.ShowDialog() == DialogResult.OK)
            ExportECB(sfd.FileName);
    }

    private void ImportECB(string fileName)
    {
        long fileSize = new FileInfo(fileName).Length;

        if (fileSize == sav.GetECBFileSize())
        {
            try
            {
                sav.ImportECB(File.ReadAllBytes(fileName));
                sav.State.Edited = true;
                string BerryName = sav.EBerryName;

                Close();
                Message.ShowFileImported(TranslationStrings.ECardBerry, BerryName);
            }
            catch
            {
                Message.ShowFileReadError(TranslationStrings.ECardBerry);
            }
        }
        else
        {
            Message.ShowInvalidFileSize(fileSize, sav.GetECBFileSize());
        }
    }

    private void ExportECB(string fileName)
    {
        try
        {
            File.WriteAllBytes(fileName, ecb);

            Close();
            Message.ShowFileExported(TranslationStrings.ECardBerry, fileName);
        }
        catch
        {
            Message.ShowFileWriteError(TranslationStrings.ECardBerry);
        }
    }

    private void ECBForm_DragEnter(object sender, DragEventArgs e)
    {
        if (e is null)
            return;
        if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move))
            e.Effect = DragDropEffects.Copy;
    }

    private void ECBForm_DragDrop(object sender, DragEventArgs e)
    {
        if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] { Length: not 0 } files)
            return;
        ImportECB(files[0]);
    }
}

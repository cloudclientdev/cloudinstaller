using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace cloudinstaller; 

public partial class MainWindow {

	public MainWindow() {
		InitializeComponent();
		checkIfFileInModsFolder();
		createCloudDirectory();
	}

	private static readonly string appData =
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

	private static readonly string modsFolder =
		appData + Path.DirectorySeparatorChar +
		".minecraft" + Path.DirectorySeparatorChar +
		"mods" + Path.DirectorySeparatorChar;

	private static readonly string versionFolder =
		appData + Path.DirectorySeparatorChar +
		".minecraft" + Path.DirectorySeparatorChar +
		"versions" + Path.DirectorySeparatorChar;

	private static readonly string cloudFolder =
		appData + Path.DirectorySeparatorChar +
		".cloud" + Path.DirectorySeparatorChar;

	private const string modURL1_7 = "https://cloudmc.dev/download/cloud-1.7.jar";
	private const string modURL1_8 = "https://cloudmc.dev/download/cloud-1.8.jar";
	private const string forgeURL1_7 = "https://maven.minecraftforge.net/net/minecraftforge/forge/1.7.10-10.13.4.1614-1.7.10/forge-1.7.10-10.13.4.1614-1.7.10-installer.jar";
	private const string forgeURL1_8 = "https://maven.minecraftforge.net/net/minecraftforge/forge/1.8.9-11.15.1.2318-1.8.9/forge-1.8.9-11.15.1.2318-1.8.9-installer.jar";
	private const string forgeName1_8 = "forge-1.8.9-11.15.1.2318-1.8.9-installer.jar";
	private const string forgeName1_7 = "forge-1.7.10-10.13.4.1614-1.7.10-installer.jar";
	private const string modName1_7 = "cloud-1.7.jar";
	private const string modName1_8 = "cloud-1.8.jar";

	private string? version;
		
	private void Button1_7_10_Click(object sender, RoutedEventArgs e) {
		version = "1.7";
		createModsDirectory();
		saveFileToModsFolder();
		if (!checkForge()) {
			installForge();
		}
	}

	private void Button1_8_9_Click(object sender, RoutedEventArgs e) {
		version = "1.8";
		createModsDirectory();
		saveFileToModsFolder();
		if (!checkForge()) {
			installForge();
		}
	}

	private void ButtonExit_Click(object sender, RoutedEventArgs e) {
		Environment.Exit(1);
	}

	private void saveFileToModsFolder() {
		using WebClient client = new WebClient();
		try {
			if (version.Contains("1.7")) {
				client.DownloadFileAsync(new Uri(modURL1_7), modsFolder + modName1_7);
				updateStatus("Downloaded " + modName1_7 + " to " + modsFolder);
			}
			else if (version.Contains("1.8")) {
				client.DownloadFileAsync(new Uri(modURL1_8), modsFolder + modName1_8);
				updateStatus("Downloaded " + modName1_8 + " to " + modsFolder);
			}
		}
		catch (Exception) {
			updateStatus("Unable to download Client");
		}
	}

	private bool checkForge() {
		string[] versions = Directory.GetDirectories(versionFolder);
		for (int i = 0; i < versions.Length; i++) {
			if (versions[i].ToLower().Contains("forge") && versions[i].ToLower().Contains(version)) {
				updateStatus("Forge installation already found");
				return true;
			}
		}

		updateStatus("Forge installation not found");
		return false;
	}

	private void installForge() {
		downloadForge();
	}

	private void downloadForge() {
		using WebClient client = new WebClient();
		try {
			if (version.Contains("1.7")) {
				client.DownloadFileAsync(new Uri(forgeURL1_7), cloudFolder + forgeName1_7);
				updateStatus("Downloaded " + forgeName1_7 + " to " + cloudFolder);
			}
			else if (version.Contains("1.8")) {
				client.DownloadFileAsync(new Uri(forgeURL1_8), cloudFolder + forgeName1_8);
				updateStatus("Downloaded " + forgeName1_8 + " to " + cloudFolder);
			}

			client.DownloadFileCompleted += runForge!;
		}
		catch (Exception) {
			updateStatus("Unable to download Forge");
		}
	}

	private void runForge(object sender, AsyncCompletedEventArgs e) {
		Process process = new Process();
		try {
			process.StartInfo.FileName = "javaw.exe";
			if (version.Contains("1.7")) {
				process.StartInfo.Arguments = "-jar " + cloudFolder + forgeName1_7;
				updateStatus("Run " + forgeName1_7);
			}
			else if (version.Contains("1.8")) {
				process.StartInfo.Arguments = "-jar " + cloudFolder + forgeName1_8;
				updateStatus("Run " + forgeName1_8);
			}

			process.Start();
		}
		catch (Exception) {
			updateStatus("Unable to run Forge");
		}
	}

	private void createCloudDirectory() {
		if (!Directory.Exists(cloudFolder)) {
			Directory.CreateDirectory(cloudFolder);
			updateStatus("Created .Cloud directory at " + cloudFolder);
		}
	}

	private void createModsDirectory() {
		if (!Directory.Exists(modsFolder)) {
			Directory.CreateDirectory(modsFolder);
			updateStatus("Created mods directory at " + modsFolder);
		}
	}

	private void checkIfFileInModsFolder() {
		if (File.Exists(modsFolder + modName1_7)) {
			Button1_7_10.Content = "Update 1.7.10";
			updateStatus("Found 1.7.10 Version already installed");
		}

		if (File.Exists(modsFolder + modName1_8)) {
			Button1_8_9.Content = "Update 1.8.9";
			updateStatus("Found 1.8.9 Version already installed");
		}
	}

	private void updateStatus(string text) {
		StatusText.Content = text;
		ListBox.Items.Add(text);
	}

	private void TopRectangle_MouseDown(object sender, MouseButtonEventArgs e) {
		if (e.ChangedButton == MouseButton.Left) {
			DragMove();
		}
	}
}
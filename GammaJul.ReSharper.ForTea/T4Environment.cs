﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.Util;
using JetBrains.VsIntegration.Application;
using Microsoft.Win32;
using PlatformID = JetBrains.ProjectModel.PlatformID;

namespace GammaJul.ReSharper.ForTea {

	/// <summary>
	/// Contains environment-dependent information.
	/// </summary>
	[ShellComponent]
	public class T4Environment {

		private readonly IVsEnvironmentInformation _vsEnvironmentInformation;
		private readonly PlatformID _platformID;
		private readonly string[] _textTemplatingAssemblyNames;
		private readonly bool _isSupported;
		private IList<FileSystemPath> _includePaths;

		/// <summary>
		/// Gets the version of the Visual Studio we're running under, two components only, <c>Major.Minor</c>. Example: “8.0”.
		/// </summary>
		[NotNull]
		public Version VsVersion2 {
			get { return _vsEnvironmentInformation.VsVersion2; }
		}

		[NotNull]
		public PlatformID PlatformID {
			get {
				if (!_isSupported)
					throw new NotSupportedException("Unsupported environment.");
				return _platformID;
			}
		}

		[NotNull]
		public IEnumerable<string> TextTemplatingAssemblyNames {
			get {
				if (!_isSupported)
					throw new NotSupportedException("Unsupported environment.");
				return _textTemplatingAssemblyNames;
			}
		}

		public bool IsSupported {
			get { return _isSupported; }
		}

		[NotNull]
		public IEnumerable<FileSystemPath> IncludePaths {
			get {
				if (!_isSupported)
					return EmptyList<FileSystemPath>.InstanceList;
				return _includePaths ?? (_includePaths = ReadIncludePaths());
			}
		}

		[NotNull]
		private IList<FileSystemPath> ReadIncludePaths() {
			string registryKey = VsEnvironmentInformation.Discovery.GetVisualStudioRegistryPath(_vsEnvironmentInformation.VsHive)
				+ @"_Config\TextTemplating\IncludeFolders\.tt";
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey)) {

				if (key == null)
					return EmptyList<FileSystemPath>.InstanceList;

				string[] valueNames = key.GetValueNames();
				if (valueNames.Length == 0)
					return EmptyList<FileSystemPath>.InstanceList;

				var paths = new List<FileSystemPath>(valueNames.Length);
				foreach (string valueName in valueNames) {
					var value = key.GetValue(valueName) as string;
					if (String.IsNullOrEmpty(value))
						continue;

					var path = new FileSystemPath(value);
					if (!path.IsEmpty && path.IsAbsolute)
						paths.Add(path);
				}
				return paths;
			}
		}

		public T4Environment([NotNull] IVsEnvironmentInformation vsEnvironmentInformation) {
			_vsEnvironmentInformation = vsEnvironmentInformation;
			
			int vsMajorVersion = vsEnvironmentInformation.VsVersion2.Major;
			if (vsMajorVersion == 10) {
				_platformID = new PlatformID(FrameworkIdentifier.NetFramework, new Version(4, 0));
				_textTemplatingAssemblyNames = new[] {
					"Microsoft.VisualStudio.TextTemplating.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
					"Microsoft.VisualStudio.TextTemplating.Interfaces.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
				};
				_isSupported = true;
			}
			else if (vsMajorVersion == 11) {
				_platformID = new PlatformID(FrameworkIdentifier.NetFramework, new Version(4, 5));
				_textTemplatingAssemblyNames = new[] {
					"Microsoft.VisualStudio.TextTemplating.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
					"Microsoft.VisualStudio.TextTemplating.Interfaces.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
					"Microsoft.VisualStudio.TextTemplating.Interfaces.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
				};
				_isSupported = true;
			}
			else
				_isSupported = false;
		}

	}

}
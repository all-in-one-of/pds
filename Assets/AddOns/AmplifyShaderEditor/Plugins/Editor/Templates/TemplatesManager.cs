// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEditor;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateInputData
	{
		public string PortName;
		public WirePortDataType DataType;
		public MasterNodePortCategory PortCategory;
		public int PortUniqueId;
		public int OrderId;
		public int TagStartIdx;
		public string TagId;
		public string DefaultValue;

		public TemplateInputData( int tagStartIdx, string tagId, string portName, string defaultValue, WirePortDataType dataType, MasterNodePortCategory portCategory, int portUniqueId, int orderId )
		{
			DefaultValue = defaultValue;
			PortName = portName;
			DataType = dataType;
			PortCategory = portCategory;
			PortUniqueId = portUniqueId;
			OrderId = orderId;
			TagId = tagId;
			TagStartIdx = tagStartIdx;
		}

		public TemplateInputData( TemplateInputData other )
		{
			DefaultValue = other.DefaultValue;
			PortName = other.PortName;
			DataType = other.DataType;
			PortCategory = other.PortCategory;
			PortUniqueId = other.PortUniqueId;
			OrderId = other.OrderId;
			TagId = other.TagId;
			TagStartIdx = other.TagStartIdx;
		}
	}

	[Serializable]
	public class TemplateProperty
	{
		public string Indentation;
		public bool UseCustomIndentation;
		public string Id;
		public bool AutoLineFeed;
		public bool Used;
		
		public TemplateProperty( string id, string indentation , bool useCustomIndentation )
		{
			Id = id;
			Indentation = indentation;
			UseCustomIndentation = useCustomIndentation;
			AutoLineFeed = !string.IsNullOrEmpty( indentation );
			Used = false;
		}
	}

	[Serializable]
	public class TemplateFunctionData
	{
		public string Id;
		public int Position;
		public string InVarType;
		public string InVarName;
		public string OutVarType;
		public string OutVarName;
		public MasterNodePortCategory Category;
		public TemplateFunctionData( string id, int position, string inVarInfo, string outVarInfo, MasterNodePortCategory category )
		{
			Id = id;
			Position = position;
			{
				string[] inVarInfoArr = inVarInfo.Split( IOUtils.VALUE_SEPARATOR );
				if ( inVarInfoArr.Length > 1 )
				{
					InVarType = inVarInfoArr[ 1 ];
					InVarName = inVarInfoArr[ 0 ];
				}
			}
			{
				string[] outVarInfoArr = outVarInfo.Split( IOUtils.VALUE_SEPARATOR );
				if ( outVarInfoArr.Length > 1 )
				{
					OutVarType = outVarInfoArr[ 1 ];
					OutVarName = outVarInfoArr[ 0 ];
				}
			}
			Category = category;
		}
	}

	[Serializable]
	public class TemplateTagData
	{
		public int StartIdx;
		public string Id;
		public bool SearchIndentation;
		public string CustomIndentation;


		public TemplateTagData( int startIdx, string id, bool searchIndentation )
		{
			StartIdx = startIdx;
			Id = id;
			SearchIndentation = searchIndentation;
			CustomIndentation = string.Empty;
		}

		public TemplateTagData( string id, bool searchIndentation)
		{
			Id = id;
			SearchIndentation = searchIndentation;
			CustomIndentation = string.Empty;
		}

		public TemplateTagData( string id, bool searchIndentation, string customIndentation )
		{
			Id = id;
			SearchIndentation = searchIndentation;
			CustomIndentation = customIndentation;
		}
	}

	public enum TemplatePortIds
	{
		Name = 0,
		DataType,
		UniqueId,
		OrderId,
	}

	public enum TemplateCommonTagId
	{
		Property	= 0,
		Global		= 1,
		Function	= 2,
		Tag			= 3,
		Pragmas		= 4,
		Pass		= 5,
		Params_Vert = 6,
		Params_Frag = 7
		//CullMode	= 8,
		//BlendMode   = 9,
		//BlendOp		= 10,
		//ColorMask	= 11,
		//StencilOp	= 12
	}

	public class TemplatesManager
	{
        public static bool Initialized = false; 
        public static readonly string TemplateShaderNameBeginTag = "/*ase_name*/";

		//public static readonly string TemplateLocalVarTag = "/*ase_local_var*/";

		public static readonly string TemplateLocalVarTag = "/*ase_local_var*/";
		public static readonly string TemplatePragmaTag = "/*ase_pragma*/";
		public static readonly string TemplatePassTag = "/*ase_pass*/";
		public static readonly string TemplatePropertyTag = "/*ase_props*/\n";
		public static readonly string TemplateGlobalsTag = "/*ase_globals*/\n";
		public static readonly string TemplateInterpolatorBeginTag = "/*ase_interp(";
		public static readonly string TemplateVertexDataTag = "/*ase_vdata:";

		public static readonly string TemplateFunctionsTag = "/*ase_functions*/\n";
		//public static readonly string TemplateTagsTag = "/*ase_tags*/";

		//public static readonly string TemplateCullModeTag = "/*ase_cull_mode*/";
		//public static readonly string TemplateBlendModeTag = "/*ase_blend_mode*/";
		//public static readonly string TemplateBlendOpTag = "/*ase_blend_op*/";
		//public static readonly string TemplateColorMaskTag = "/*ase_color_mask*/";
		//public static readonly string TemplateStencilOpTag = "/*ase_stencil*/";

		public static readonly string TemplateCodeSnippetAttribBegin = "#CODE_SNIPPET_ATTRIBS_BEGIN#";
		public static readonly string TemplateCodeSnippetAttribEnd = "#CODE_SNIPPET_ATTRIBS_END#\n";
		public static readonly string TemplateCodeSnippetEnd = "#CODE_SNIPPET_END#\n";

		public static readonly char TemplateNewLine = '\n';

		// INPUTS AREA
		public static readonly string TemplateInputsVertBeginTag = "/*ase_vert_out:";
		public static readonly string TemplateInputsFragBeginTag = "/*ase_frag_out:";
		public static readonly string TemplateInputsVertParamsTag = "/*ase_vert_input*/";
		public static readonly string TemplateInputsFragParamsTag = "/*ase_frag_input*/";
		

		// CODE AREA
		public static readonly string TemplateVertexCodeBeginArea = "/*ase_vert_code:";
		public static readonly string TemplateFragmentCodeBeginArea = "/*ase_frag_code:";

		
		public static readonly string TemplateEndOfLine = "*/\n";
		public static readonly string TemplateEndSectionTag = "*/";
		public static readonly string TemplateFullEndTag = "/*end*/";

		public static readonly string NameFormatter = "\"{0}\"";

		public static readonly TemplateTagData[] CommonTags = { new TemplateTagData( TemplatePropertyTag,true),
																new TemplateTagData( TemplateGlobalsTag,true),
																new TemplateTagData( TemplateFunctionsTag,true),
																//new TemplateTagData( TemplateTagsTag,false," "),
																new TemplateTagData( TemplatePragmaTag,true),
																new TemplateTagData( TemplatePassTag,true),
																new TemplateTagData( TemplateInputsVertParamsTag,false),
																new TemplateTagData( TemplateInputsFragParamsTag,false)
																//new TemplateTagData( TemplateCullModeTag,false),
																//new TemplateTagData( TemplateBlendModeTag,false),
																//new TemplateTagData( TemplateBlendOpTag,false),
																//new TemplateTagData( TemplateColorMaskTag,false),
																//new TemplateTagData( TemplateStencilOpTag,true),
																};

		
		private static Dictionary<string, TemplateData> m_availableTemplates;

		private static List<TemplateData> m_sortedTemplates;
		public static string[] AvailableTemplateNames;

		public static void Init()
		{
			if( !Initialized )
			{
				m_availableTemplates = new Dictionary<string, TemplateData>();
				m_sortedTemplates = new List<TemplateData>();

				// Post-Process
				{
					TemplateData postProcess = new TemplateData( "Post Process", "c71b220b631b6344493ea3cf87110c93" );
					if( postProcess.IsValid )
						AddTemplate( postProcess );
				}

				// Default Unlit
				{
					TemplateData defaultUnlit = new TemplateData( "Default Unlit", "6e114a916ca3e4b4bb51972669d463bf" );
					if( defaultUnlit.IsValid )
						AddTemplate( defaultUnlit );
				}

				// UI
				{
					TemplateData defaultUI = new TemplateData( "Default UI", "5056123faa0c79b47ab6ad7e8bf059a4" );
					if( defaultUI.IsValid )
						AddTemplate( defaultUI );
				}

				// Sprites
				{
					TemplateData defaultSprites = new TemplateData( "Default Sprites", "0f8ba0101102bb14ebf021ddadce9b49" );
					if( defaultSprites.IsValid )
						AddTemplate( defaultSprites );
				}

				// Particles
				{
					TemplateData particlesAlphaBlended = new TemplateData( "Particles Alpha Blended", "0b6a9f8b4f707c74ca64c0be8e590de0" );
					if( particlesAlphaBlended.IsValid )
						AddTemplate( particlesAlphaBlended );
				}

				// Particles Cutout
				//{
				//	TemplateData particlesCutout = new TemplateData( "Particles Cutout", "9dc0487e4f202ba49bbef443f0ddf5ca" );
				//	if( particlesCutout.IsValid )
				//		AddTemplate( particlesCutout );
				//}


				// Search for other possible templates on the project
				string[] allShaders = AssetDatabase.FindAssets( "t:shader" );
				for( int i = 0; i < allShaders.Length; i++ )
				{
					if( !m_availableTemplates.ContainsKey( allShaders[ i ] ) )
					{
						CheckAndLoadTemplate( allShaders[ i ] );
					}
				}

				// TODO: Sort list alphabeticaly 
				AvailableTemplateNames = new string[ m_sortedTemplates.Count + 1 ];
				AvailableTemplateNames[ 0 ] = "Custom";
				for( int i = 0; i < m_sortedTemplates.Count; i++ )
				{
					m_sortedTemplates[ i ].OrderId = i;
					AvailableTemplateNames[ i + 1 ] = m_sortedTemplates[ i ].Name;
				}
				Initialized = true;
			}
        }



		public static void CreateTemplateMenuItems()
		{
			if ( m_sortedTemplates == null || m_sortedTemplates.Count == 0 )
				return;

			System.Text.StringBuilder fileContents = new System.Text.StringBuilder();
			fileContents.Append( "// Amplify Shader Editor - Visual Shader Editing Tool\n" );
			fileContents.Append( "// Copyright (c) Amplify Creations, Lda <info@amplify.pt>\n" );
			fileContents.Append( "using UnityEditor;\n" );
			fileContents.Append( "\n" );
			fileContents.Append( "namespace AmplifyShaderEditor\n" );
			fileContents.Append( "{\n" );
			fileContents.Append( "\tpublic class TemplateMenuItems\n" );
			fileContents.Append( "\t{\n" );
			int fixedPriority = 85;
			for ( int i = 0; i < m_sortedTemplates.Count; i++ )
			{
				fileContents.AppendFormat( "\t\t[ MenuItem( \"Assets/Create/Amplify Shader/{0}\", false, {1} )]\n", m_sortedTemplates[ i ].Name, fixedPriority );
				fileContents.AppendFormat( "\t\tpublic static void ApplyTemplate{0}()\n", i );
				fileContents.Append( "\t\t{\n" );
				fileContents.AppendFormat( "\t\t\tAmplifyShaderEditorWindow.CreateNewTemplateShader( \"{0}\" );\n", m_sortedTemplates[ i ].GUID );
				fileContents.Append( "\t\t}\n" );
			}
			fileContents.Append( "\t}\n" );
			fileContents.Append( "}\n" );
			string filePath = AssetDatabase.GUIDToAssetPath( "da0b931bd234a1e43b65f684d4b59bfb" );
			IOUtils.SaveTextfileToDisk( fileContents.ToString(), filePath, false );
			AssetDatabase.ImportAsset( filePath );
		}

		public static int GetIdForTemplate( TemplateData templateData )
		{
			if ( templateData == null )
				return -1;

			for ( int i = 0; i < m_sortedTemplates.Count; i++ )
			{
				if ( m_sortedTemplates[ i ].GUID.Equals( templateData.GUID ) )
					return m_sortedTemplates[ i ].OrderId;
			}
			return -1;
		}

		public static void AddTemplate( TemplateData templateData )
		{
			if ( templateData == null )
				return;

			if ( !m_availableTemplates.ContainsKey( templateData.GUID ) )
			{
				m_sortedTemplates.Add( templateData );
				m_availableTemplates.Add( templateData.GUID, templateData );
			}
		}

		public static void RemoveTemplate( string guid )
		{
			TemplateData templateData = GetTemplate( guid );
			if ( templateData != null )
			{
				RemoveTemplate( templateData );
			}
		}

		public static void RemoveTemplate( TemplateData templateData )
		{
			if( m_availableTemplates != null )
				m_availableTemplates.Remove( templateData.GUID );

			m_sortedTemplates.Remove( templateData );
			templateData.Destroy();
		}
		
		public static void Destroy()
		{
			if ( m_availableTemplates != null )
			{
				foreach ( KeyValuePair<string, TemplateData> kvp in m_availableTemplates )
				{
					kvp.Value.Destroy();
				}
				m_availableTemplates.Clear();
				m_availableTemplates = null;
			}
			AvailableTemplateNames = null;
			Initialized = false;
		}

		public static TemplateData GetTemplate( int id )
		{
			if ( id < m_sortedTemplates.Count )
				return m_sortedTemplates[ id ];

			return null;
		}
		
		public static TemplateData GetTemplate( string guid )
		{
			if ( m_availableTemplates == null && m_sortedTemplates != null )
			{
				m_availableTemplates = new Dictionary<string, TemplateData>();
				for ( int i = 0; i < m_sortedTemplates.Count; i++ )
				{
					m_availableTemplates.Add( m_sortedTemplates[ i ].GUID, m_sortedTemplates[ i ] );
				}
			}

			if ( m_availableTemplates.ContainsKey( guid ) )
				return m_availableTemplates[ guid ];

			return null;
		}

		public static TemplateData GetTemplateByName( string name )
		{
			if( m_availableTemplates == null && m_sortedTemplates != null )
			{
				m_availableTemplates = new Dictionary<string, TemplateData>();
				for( int i = 0; i < m_sortedTemplates.Count; i++ )
				{
					m_availableTemplates.Add( m_sortedTemplates[ i ].GUID, m_sortedTemplates[ i ] );
				}
			}

			foreach( KeyValuePair<string, TemplateData> kvp in m_availableTemplates )
			{
				if( kvp.Value.DefaultShaderName.Equals( name ) )
				{
					return kvp.Value;
				}
			}
				
			return null;
		}

		public static TemplateData CheckAndLoadTemplate( string guid )
		{
			TemplateData templateData = GetTemplate( guid );
			if ( templateData == null )
			{
				string datapath = AssetDatabase.GUIDToAssetPath( guid );
				string body = IOUtils.LoadTextFileFromDisk( datapath );

				if ( body.IndexOf( TemplatesManager.TemplateShaderNameBeginTag ) > -1 )
				{
					templateData = new TemplateData( string.Empty, guid, body );
					if ( templateData.IsValid )
					{
						AddTemplate( templateData );
						return templateData;
					}
				}
			}
			return null;
		}

		//public static TemplateData LoadAndCacheTemplate( string guid )
		//{
		//	TemplateData templateData = new TemplateData( string.Empty, guid );
		//	if ( templateData.IsValid )
		//	{
		//		AddTemplate( templateData );
		//	}
		//	return templateData;
		//}

		public static int TemplateCount { get { return m_sortedTemplates.Count; } }
	}
}

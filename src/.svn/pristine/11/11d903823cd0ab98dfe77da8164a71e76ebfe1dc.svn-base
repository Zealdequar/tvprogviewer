// ------------------------------------------------------------------
// DirectX.Capture
//
// History:
//	2003-Jan-24		BL		- created
//
// Copyright (c) 2003 Brian Low
// ------------------------------------------------------------------

using System;
using System.Runtime.InteropServices; 
#if DSHOWNET
using DShowNET;
#else
using DirectShowLib;
#endif

namespace DirectX.Capture
{
	/// <summary>
	///  Represents a physical connector or source on an 
	///  audio/video device. This class is used on filters that
	///  support the IAMCrossbar interface such as TV Tuners.
	/// </summary>
	public class CrossbarSource : Source
	{
		// --------------------- Private/Internal properties -------------------------

		internal IAMCrossbar			Crossbar;				// crossbar filter (COM object)
		internal int					OutputPin;				// output pin number on the crossbar
		internal int					InputPin;				// input pin number on the crossbar
		internal int					RelatedInputPin = -1;	// usually the audio input pin for the same source
		internal CrossbarSource			RelatedInputSource;		// the Crossbar source associated with the RelatedInputPin
		internal PhysicalConnectorType	ConnectorType;			// type of the connector



		// ----------------------- Public properties -------------------------

		/// <summary> Enabled or disable this source. </summary>
		public override bool Enabled
		{
			get 
			{
				int i;
				if ( Crossbar.get_IsRoutedTo( OutputPin, out i ) == 0 )
					if ( InputPin == i )
						return( true );
				return( false );
			}

			set
			{
				if ( value )
				{
					// Enable this route
					int hr = this.Crossbar.Route( this.OutputPin, this.InputPin );
					if ( hr < 0 ) Marshal.ThrowExceptionForHR( hr );

					// Enable the related pin as well
					if ( RelatedInputSource != null )
					{
						hr = this.Crossbar.Route( RelatedInputSource.OutputPin, RelatedInputSource.InputPin );  
						if ( hr < 0 ) Marshal.ThrowExceptionForHR( hr );

					}
				}
				else
				{
					// Disable this route by routing the output
					// pin to input pin -1
					int hr = this.Crossbar.Route( this.OutputPin, -1 );
					if ( hr < 0 ) Marshal.ThrowExceptionForHR( hr );

					// Disable the related pin as well
					if ( RelatedInputSource != null )
					{
						hr = this.Crossbar.Route( RelatedInputSource.OutputPin, -1 );  
						if ( hr < 0 ) Marshal.ThrowExceptionForHR( hr );

					}
				}
			}
		}


		
		// -------------------- Constructors/Destructors ----------------------

		/// <summary> Constructor. This class cannot be created directly. </summary>
		internal CrossbarSource( IAMCrossbar crossbar, int outputPin, int inputPin, PhysicalConnectorType connectorType )
		{
			this.Crossbar = crossbar;
			this.OutputPin = outputPin;
			this.InputPin = inputPin;
			this.ConnectorType = connectorType;
			this.name = getName( connectorType );
		}

		/// <summary> Constructor. This class cannot be created directly. </summary>
		internal CrossbarSource( IAMCrossbar crossbar, int outputPin, int inputPin, int relatedInputPin, PhysicalConnectorType connectorType )
		{
			this.Crossbar = crossbar;
			this.OutputPin = outputPin;
			this.InputPin = inputPin;
			this.RelatedInputPin = relatedInputPin; 
			this.ConnectorType = connectorType;
			this.name = getName( connectorType );
		}

		// --------------------------- Private methods ----------------------------

		/// <summary> Retrieve the friendly name of a connectorType. </summary>
		private string getName( PhysicalConnectorType connectorType )
		{
			string name;
			switch( connectorType )
			{
				case PhysicalConnectorType.Video_Tuner:				name = "Видео Тюнер";			break;
				case PhysicalConnectorType.Video_Composite:			name = "Видео Композитный";		break;
				case PhysicalConnectorType.Video_SVideo:			name = "Видео S-Video";			break;
				case PhysicalConnectorType.Video_RGB:				name = "Видео RGB";				break;
				case PhysicalConnectorType.Video_YRYBY:				name = "Видео YRYBY";			break;
				case PhysicalConnectorType.Video_SerialDigital:		name = "Видео Цифровой Последовательный";	break;
				case PhysicalConnectorType.Video_ParallelDigital:	name = "Видео Цифровой Параллельный";break;
				case PhysicalConnectorType.Video_SCSI:				name = "Видео SCSI";			break;
				case PhysicalConnectorType.Video_AUX:				name = "Видео AUX";				break;
				case PhysicalConnectorType.Video_1394:				name = "Видео Firewire";		break;
				case PhysicalConnectorType.Video_USB:				name = "Видео USB";				break;
				case PhysicalConnectorType.Video_VideoDecoder:		name = "Видео Декодер";			break;
				case PhysicalConnectorType.Video_VideoEncoder:		name = "Видео Энкодер";			break;
				case PhysicalConnectorType.Video_SCART:				name = "Видео SCART";			break;

				case PhysicalConnectorType.Audio_Tuner:				name = "Аудио Тюнер";			break;
				case PhysicalConnectorType.Audio_Line:				name = "Аудио Вход";			break;
				case PhysicalConnectorType.Audio_Mic:				name = "Аудио Микрофон";				break;
				case PhysicalConnectorType.Audio_AESDigital:		name = "Аудио AES Цифровой";		break;
				case PhysicalConnectorType.Audio_SPDIFDigital:		name = "Аудио SPDIF Цифровой";	break;
				case PhysicalConnectorType.Audio_SCSI:				name = "Аудио SCSI";			break;
				case PhysicalConnectorType.Audio_AUX:				name = "Аудио AUX";				break;
				case PhysicalConnectorType.Audio_1394:				name = "Аудио Firewire";		break;
				case PhysicalConnectorType.Audio_USB:				name = "Аудио USB";				break;
				case PhysicalConnectorType.Audio_AudioDecoder:		name = "Аудио Декодер";			break;

				default:											name = "Неизвестное Подключение";		break;
			}
			return( name );
		}


		
		// -------------------- IDisposable -----------------------

		/// <summary> Release unmanaged resources. </summary>
		public override void Dispose()
		{
			if ( Crossbar != null )
				Marshal.ReleaseComObject( Crossbar );
			Crossbar = null;
			RelatedInputSource = null;
			base.Dispose();
		}	
	}
}

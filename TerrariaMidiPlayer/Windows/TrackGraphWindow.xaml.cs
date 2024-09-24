using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TerrariaMidiPlayer.Controls;
using TerrariaMidiPlayer.Util;

namespace TerrariaMidiPlayer.Windows {
	/// <summary>
	/// Interaction logic for TrackGraphWindow.xaml
	/// </summary>
	public partial class TrackGraphWindow : Window {
		//=========== MEMBERS ============
		#region Members

		/**<summary>The saved width of the window.</summary>*/
		private static double InitialWidth = 0;
		/**<summary>The saved height of the window.</summary>*/
		private static double InitialHeight = 0;

		/**<summary>The current track index.</summary>*/
		int trackIndex;
		/**<summary>True if the window is loaded.</summary>*/
		bool loaded = false;
		/**<summary>The playback UI update timer.</summary>*/
		Timer playbackUITimer = new Timer(100);
		/**<summary>The old piano mode before opening the window.</summary>*/
		bool oldPianoMode;
		/**<summary>True if changing the song position.</summary>*/
		bool dragging = false;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the track graph window.</summary>*/
		public TrackGraphWindow(Midi midi, int trackIndex) {
			InitializeComponent();

			playbackUITimer.Elapsed += OnPlaybackUIUpdate;
			playbackUITimer.AutoReset = true;
			if (!DesignerProperties.GetIsInDesignMode(this)) {
				playbackUITimer.Start();
				oldPianoMode = Config.PianoMode;
				Config.PianoMode = true;
				toggleButtonPiano.IsChecked = true;
				toggleButtonWrapPianoMode.IsChecked = Config.WrapPianoMode;
				toggleButtonSkipPianoMode.IsChecked = Config.SkipPianoMode;
			}

			ListBoxItem item = new ListBoxItem();
			item.Content = "所有轨道";
			item.FontWeight = FontWeights.Bold;
			listTracks.Items.Add(item);
			for (int i = 0; i < Config.Midi.TrackCount; i++) {
				item = new ListBoxItem();
				item.Content = Config.Midi.GetTrackSettingsAt(i).ProperName;
				if (!Config.Midi.GetTrackSettingsAt(i).Enabled)
					item.Foreground = Brushes.Gray;
				listTracks.Items.Add(item);
			}
			this.trackIndex = trackIndex;

			this.listTracks.SelectedIndex = trackIndex + 1;

			loaded = true;

			Width = InitialWidth;
			Height = InitialHeight;
			Title += " - " + midi.ProperName;
		}

		#endregion
		//============ EVENTS ============
		#region Events
		//--------------------------------
		#region Window

		private void OnWindowClosing(object sender, CancelEventArgs e) {
			InitialWidth = Width;
			InitialHeight = Height;
			Config.PianoMode = oldPianoMode;
			if (!Config.PianoMode) {
				Config.MainWindow.StopOrPause();
			}
		}
		private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e) {
			if (!dragging)
				OnPlaybackUIUpdate(null, null);
		}
		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			// Make text boxes lose focus on click away
			FocusManager.SetFocusedElement(this, this);
		}

		#endregion
		//--------------------------------
		#region View

		private void OnValidChecked(object sender, RoutedEventArgs e) {
			trackGraph.ShowValidNotes = checkBoxValid.IsChecked.Value;
			trackGraph.Update();
		}
		private void OnWrappedChecked(object sender, RoutedEventArgs e) {
			trackGraph.ShowWrappedNotes = checkBoxWrapped.IsChecked.Value;
			trackGraph.Update();
		}
		private void OnSkippedChecked(object sender, RoutedEventArgs e) {
			trackGraph.ShowSkippedNotes = checkBoxSkipped.IsChecked.Value;
			trackGraph.Update();
		}

		#endregion
		//--------------------------------
		#region Playback Position

		private void OnLeftMouseDown(object sender, MouseButtonEventArgs e) {
			dragging = true;
			OnMouseMove(sender, e);
			playArea.CaptureMouse();
		}
		private void OnLeftMouseUp(object sender, MouseButtonEventArgs e) {
			dragging = false;
			playArea.ReleaseMouseCapture();
		}
		private void OnMouseMove(object sender, MouseEventArgs e) {
			if (dragging) {
				double width = playArea.ActualWidth - 2;
				double x = Math.Max(0, Math.Min(width, e.GetPosition(playArea).X));
				double progress = x / width;
				playMarker.Margin = new Thickness(x, 0, 0, 0);
				Config.Sequencer.Position = Config.Sequencer.ProgressToTicks(progress);
				UpdatePlayButtons();
			}
		}

		#endregion
		//--------------------------------
		#region Midi/Track Settings

		private void OnTrackChanged(object sender, SelectionChangedEventArgs e) {
			if (listTracks.SelectedIndex != -1) {
				trackIndex = listTracks.SelectedIndex - 1;
				trackGraph.LoadTrack(trackIndex);
				UpdateTrack();
			}
		}
		private void OnTrackEnabledClicked(object sender, RoutedEventArgs e) {
			if (!loaded)
				return;
			loaded = false;

			Config.Midi.GetTrackSettingsAt(trackIndex).Enabled = checkBoxTrackEnabled.IsChecked.Value;

			listTracks.Items.RemoveAt(trackIndex + 1);
			ListBoxItem item = new ListBoxItem();
			item.Content = Config.Midi.GetTrackSettingsAt(trackIndex).ProperName;
			if (!Config.Midi.GetTrackSettingsAt(trackIndex).Enabled)
				item.Foreground = Brushes.Gray;
			listTracks.Items.Insert(trackIndex + 1, item);
			listTracks.SelectedIndex = trackIndex + 1;

			loaded = true;
			trackGraph.Update();
		}
		private void OnOctaveOffsetChanged(object sender, ValueChangedEventArgs<int> e) {
			if (!loaded)
				return;
			Config.Midi.GetTrackSettingsAt(trackIndex).OctaveOffset = numericOctaveOffset.Value;
			UpdateTrackNotes();
		}
		private void OnNoteOffsetChanged(object sender, ValueChangedEventArgs<int> e) {
			if (!loaded)
				return;
			Config.Midi.NoteOffset = numericNoteOffset.Value;
			UpdateTrackNotes();
		}
		private void OnSpeedChanged(object sender, ValueChangedEventArgs<int> e) {
			if (!loaded)
				return;
			Config.Midi.Speed = numericSpeed.Value;
			Config.Sequencer.Speed = Config.Midi.SpeedRatio;
            labelDuration.Content = "时长: " + MillisecondsToString(Config.Sequencer.Duration);
            trackGraph.ReloadTrack();
			trackGraph.Update();
			UpdatePlayTime();
		}

		#endregion
		//--------------------------------
		#region Playback

		private void OnPlaybackUIUpdate(object sender, ElapsedEventArgs e) {
			Dispatcher.Invoke(() => {
				if (!dragging) {
					double progress = (double)Config.Sequencer.TicksToMilliseconds(Config.Sequencer.Position) / Config.Sequencer.Duration;
					double x = progress * (playArea.ActualWidth - 2);
					playMarker.Margin = new Thickness(x, 0, 0, 0);
				}
				UpdatePlayButtons();
				UpdatePlayTime();
			});
		}
		private void OnWrapPianoMode(object sender, RoutedEventArgs e) {
			if (!loaded)
				return;
			Config.WrapPianoMode = toggleButtonWrapPianoMode.IsChecked.Value;
		}
		private void OnSkipPianoMode(object sender, RoutedEventArgs e) {
			if (!loaded)
				return;
			Config.SkipPianoMode = toggleButtonSkipPianoMode.IsChecked.Value;
		}
		private void OnPianoToggled(object sender, RoutedEventArgs e) {
			if (!loaded)
				return;
			Config.MainWindow.StopOrPause();
			Config.PianoMode = toggleButtonPiano.IsChecked.Value;
			UpdatePlayButtons();
		}
		private void OnStopToggled(object sender, RoutedEventArgs e) {
			Config.MainWindow.Stop();
			UpdatePlayButtons();
		}
		private void OnPlayToggled(object sender, RoutedEventArgs e) {
			Config.MainWindow.Play();
			UpdatePlayButtons();
		}
		private void OnPauseToggled(object sender, RoutedEventArgs e) {
			Config.MainWindow.Pause();
			UpdatePlayButtons();
		}

        #endregion
        //--------------------------------
        #endregion
        //=========== UPDATING ===========
        #region Updating

        /**<summary>Updates changes to the track.</summary>*/
        /** <summary>更新所选MIDI轨道的变化。</summary> */
        private void UpdateTrack()
        {
            // 更新轨道图表
            trackGraph.Update();

            // 启用或禁用复选框和数值控件，取决于是否有选中的轨道
            checkBoxTrackEnabled.IsEnabled = trackIndex != -1;
            numericOctaveOffset.IsEnabled = trackIndex != -1;

            // 设置全局速度和音符偏移
            numericSpeed.Value = Config.Midi.Speed;
            numericNoteOffset.Value = Config.Midi.NoteOffset;

            // 更新UI标签显示和弦数
            labelChords.Content = "和弦: " + trackGraph.Chords;
            // 更新UI标签显示总时长
            labelDuration.Content = "时长: " + MillisecondsToString(Config.Sequencer.Duration);
            // 更新UI标签显示总音符数
            labelNotes.Content = "音符: " + trackGraph.TotalNotes;

            // 如果有选中的轨道
            if (trackIndex != -1)
            {
                // 设置当前轨道的八度偏移
                numericOctaveOffset.Value = Config.Midi.GetTrackSettingsAt(trackIndex).OctaveOffset;
                // 设置当前轨道是否启用
                checkBoxTrackEnabled.IsChecked = Config.Midi.GetTrackSettingsAt(trackIndex).Enabled;
            }

            // 更新工具提示，显示有效音符数
            checkBoxValid.ToolTip = trackGraph.ValidNotes + " 个音符";
            // 更新工具提示，显示包裹音符数
            checkBoxWrapped.ToolTip = trackGraph.WrappedNotes + " 个音符";
            // 更新工具提示，显示跳过音符数
            checkBoxSkipped.ToolTip = trackGraph.SkippedNotes + " 个音符";

            // 更新轨道的音符范围
            UpdateTrackNotes();
        }
        /**<summary>Updates changes to the midi track's note range.</summary>*/
        /** <summary>更新MIDI轨道的音符范围。</summary> */
        private void UpdateTrackNotes()
        {
            // 如果没有选中的轨道
            if (trackIndex == -1)
            {
                // 初始化最高音和最低音
                int highestNote = 0;
                int lowestNote = 132; // MIDI音符编号范围是0到127，这里设为132是为了确保第一次比较时会被替换

                // 遍历所有轨道
                for (int index = 0; index < Config.Midi.TrackCount; index++)
                {
                    // 如果当前轨道被禁用，则跳过
                    if (!Config.Midi.GetTrackSettingsAt(index).Enabled)
                        continue;

                    // 更新最高音
                    highestNote = Math.Max(highestNote, Config.Midi.GetTrackAt(index).HighestNote);
                    // 更新最低音
                    lowestNote = Math.Min(lowestNote, Config.Midi.GetTrackAt(index).LowestNote);
                }

                // 如果最低音高于最高音（这种情况通常不应该发生），则设置默认值
                if (lowestNote > highestNote)
                {
                    lowestNote = 12;
                    highestNote = 12;
                }

                // 更新UI标签显示最高音
                labelHighestNote.Content = "最高音: " + NoteToString(
                    highestNote + Config.Midi.NoteOffset
                );
                // 更新UI标签显示最低音
                labelLowestNote.Content = "最低音: " + NoteToString(
                    lowestNote + Config.Midi.NoteOffset
                );
            }
            else
            {
                // 如果有选中的轨道
                // 更新UI标签显示最高音
                labelHighestNote.Content = "最高音: " + NoteToString(
                    Config.Midi.GetTrackAt(trackIndex).HighestNote +
                    Config.Midi.NoteOffset
                );
                // 更新UI标签显示最低音
                labelLowestNote.Content = "最低音: " + NoteToString(
                    Config.Midi.GetTrackAt(trackIndex).LowestNote +
                    Config.Midi.NoteOffset
                );
            }
        }
        /**<summary>Updates the play time in the playback tab.</summary>*/
        private void UpdatePlayTime() {
			labelMidiPosition.Content = MillisecondsToString(Config.Sequencer.CurrentTime) + "/" + MillisecondsToString(Config.Sequencer.Duration);
		}
		/**<summary>Updates enabled state of midi buttons.</summary>*/
		private void UpdatePlayButtons() {
			toggleButtonStop.IsChecked = Config.Sequencer.Position <= 1 && !Config.Sequencer.IsPlaying;
			toggleButtonPlay.IsChecked = Config.Sequencer.IsPlaying;
			toggleButtonPause.IsChecked = Config.Sequencer.Position > 1 && !Config.Sequencer.IsPlaying;
		}

		#endregion
		//=========== HELPERS ============
		#region Helpers

		/**<summary>Gets the name of the specified note.</summary>*/
		private string NoteToString(int note) {
			string[] notes = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
			string[] flatNotes = { "", "D\u266D", "", "E\u266D", "", "", "G\u266D", "", "A\u266D", "", "B\u266D", "" };
			int semitone = note % 12;
			note -= 12;
			string noteStr = notes[semitone] + (note / 12);
			if (flatNotes[semitone].Length > 0)
				noteStr += " (" + flatNotes[semitone] + (note / 12) + ")";
			return noteStr;
		}
		/**<summary>Converts milliseconds to a string.</summary>*/
		private string MillisecondsToString(int milliseconds, bool showHours = false, bool showMilliseconds = false) {
			int ms = milliseconds % 1000;
			int seconds = (milliseconds / 1000) % 60;
			int minutes = (milliseconds / 1000 / 60);
			int hours = (milliseconds / 1000 / 60 / 60);
			if (showHours)
				minutes %= 60;

			string timeStr = "";
			if (showHours) {
				timeStr += hours.ToString() + ":";
				if (minutes < 10)
					timeStr += "0";
			}
			timeStr += minutes.ToString() + ":";
			if (seconds < 10)
				timeStr += "0";
			timeStr += seconds.ToString();
			if (showMilliseconds) {
				timeStr += "." + ms.ToString();
			}
			return timeStr;
		}

		#endregion
		//=========== SHOWING ============
		#region Showing

		/**<summary>Shows the track graph window.</summary>*/
		public static void Show(Window owner, Midi midi, int trackIndex) {
			TrackGraphWindow window = new TrackGraphWindow(midi, trackIndex);
			window.Owner = owner;
			window.ShowDialog();
		}

		#endregion
	}
}

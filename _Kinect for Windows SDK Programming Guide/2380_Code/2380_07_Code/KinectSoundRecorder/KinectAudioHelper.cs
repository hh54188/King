
namespace KinectSoundRecorder
{
    using System.IO;
    using System;
    using Microsoft.Kinect;
    using System.Text;

    /// <summary>
    /// Kinect Audio Recorder Helper
    /// </summary>
    public class KinectAudioHelper
    {
        /// <summary>
        /// The sensor object
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectAudioHelper"/> class.
        /// </summary>
        /// <param name="sensor">The sensor.</param>
        public KinectAudioHelper(KinectSensor sensor)
        {
            if (sensor != null)
            {
                this.sensor = sensor;
            }
        }

        struct WAVEFORMATEX
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;
        }

        /// <summary>
        /// A bare bones WAV file header writer
        /// </summary>        
        static void WriteWavHeader(Stream stream, int dataLength)
        {
            //We need to use a memory stream because the BinaryWriter will close the underlying stream when it is closed
            using (var memStream = new MemoryStream(64))
            {
                int cbFormat = 18; //sizeof(WAVEFORMATEX)
                WAVEFORMATEX format = new WAVEFORMATEX()
                {
                    wFormatTag = 1,
                    nChannels = 1,
                    nSamplesPerSec = 16000,
                    nAvgBytesPerSec = 32000,
                    nBlockAlign = 2,
                    wBitsPerSample = 16,
                    cbSize = 0
                };

                using (var binarywriter = new BinaryWriter(memStream))
                {
                    //RIFF header
                    WriteString(memStream, "RIFF");
                    binarywriter.Write(dataLength + cbFormat + 4); //File size - 8
                    WriteString(memStream, "WAVE");
                    WriteString(memStream, "fmt ");
                    binarywriter.Write(cbFormat);

                    //WAVEFORMATEX
                    binarywriter.Write(format.wFormatTag);
                    binarywriter.Write(format.nChannels);
                    binarywriter.Write(format.nSamplesPerSec);
                    binarywriter.Write(format.nAvgBytesPerSec);
                    binarywriter.Write(format.nBlockAlign);
                    binarywriter.Write(format.wBitsPerSample);
                    binarywriter.Write(format.cbSize);

                    //data header
                    WriteString(memStream, "data");
                    binarywriter.Write(dataLength);
                    memStream.WriteTo(stream);
                }
            }
        }

        /// <summary>
        /// Writes the string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="s">The s.</param>
        static void WriteString(Stream stream, string s)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            stream.Write(bytes, 0, bytes.Length);
        }
        /// <summary>
        /// Records the audio.
        /// </summary>
        public void RecordAudio()
        {
            int recordingLength = (int)5 * 2 * 16000;
            byte[] buffer = new byte[1024];

            using (FileStream _fileStream = new FileStream("d:\\kinectAudio.wav", FileMode.Create))
            {
                WriteWavHeader(_fileStream, recordingLength);

               
                //Start capturing audio                               
                using (Stream audioStream = this.sensor.AudioSource.Start())
                {
                    //Simply copy the data from the stream down to the file
                    int count, totalCount = 0;
                    while ((count = audioStream.Read(buffer, 0, buffer.Length)) > 0 && totalCount < recordingLength)
                    {
                        _fileStream.Write(buffer, 0, count);
                        totalCount += count;


                    }
                }
            }

        }
    }
}
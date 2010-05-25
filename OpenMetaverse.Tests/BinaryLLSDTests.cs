/*
 * Copyright (c) 2007-2008, openmetaverse.org
 * All rights reserved.
 *
 * - Redistribution and use in source and binary forms, with or without
 *   modification, are permitted provided that the following conditions are met:
 *
 * - Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 * - Neither the name of the openmetaverse.org nor the names
 *   of its contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

/* 
 * 
 * This tests are based upon the description at
 * 
 * http://wiki.secondlife.com/wiki/LLSD
 * 
 * and (partially) generated by the (supposed) reference implementation at
 * 
 * http://svn.secondlife.com/svn/linden/release/indra/lib/python/indra/base/llsd.py
 * 
 */

using System;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using OpenMetaverse.StructuredData;

namespace OpenMetaverse.Tests
{

    [TestFixture()]
    public class BinarySDTests
    {
        private static byte[] binaryHead = { 0x3c, 0x3f, 0x6c, 0x6c, 0x73, 0x64, 0x2f, 0x62, 0x69, 0x6e, 0x61, 0x72, 0x79, 0x3f, 0x3e, 0xa };

        [Test()]
        public void HelperFunctions()
        {
            string s = "this is a teststring so that we can find something from the beginning";
            byte[] sBinary = Encoding.ASCII.GetBytes(s);
            MemoryStream stream = new MemoryStream(sBinary);

            stream.Position = 0L;
            bool result = OSDParser.FindString(stream, "this");
            Assert.AreEqual(true, result);
            Assert.AreEqual(4L, stream.Position);

            stream.Position = 10L;
            result = OSDParser.FindString(stream, "teststring");
            Assert.AreEqual(true, result);
            Assert.AreEqual(20L, stream.Position);

            stream.Position = 25L;
            result = OSDParser.FindString(stream, "notfound");
            Assert.AreEqual(false, result);
            Assert.AreEqual(25L, stream.Position);

            stream.Position = 60L;
            result = OSDParser.FindString(stream, "beginningAndMore");
            Assert.AreEqual(false, result);
            Assert.AreEqual(60L, stream.Position);

            byte[] sFrontWhiteSpace = Encoding.ASCII.GetBytes("   \t\t\n\rtest");
            MemoryStream streamTwo = new MemoryStream(sFrontWhiteSpace);
            OSDParser.SkipWhiteSpace(streamTwo);
            Assert.AreEqual(7L, streamTwo.Position);

            byte[] sMiddleWhiteSpace = Encoding.ASCII.GetBytes("test \t\t\n\rtest");
            MemoryStream streamThree = new MemoryStream(sMiddleWhiteSpace);
            streamThree.Position = 4L;
            OSDParser.SkipWhiteSpace(streamThree);
            Assert.AreEqual(9L, streamThree.Position);

            byte[] sNoWhiteSpace = Encoding.ASCII.GetBytes("testtesttest");
            MemoryStream streamFour = new MemoryStream(sNoWhiteSpace);
            OSDParser.SkipWhiteSpace(streamFour);
            Assert.AreEqual(0L, streamFour.Position);

        }

        // Testvalues for Undef:
        private static byte[] binaryUndefValue = { 0x21 };
        private static byte[] binaryUndef = (byte[])ConcatenateArrays(binaryHead, binaryUndefValue);

        [Test()]
        public void DeserializeUndef()
        {
            OSD llsdUndef = OSDParser.DeserializeLLSDBinary(binaryUndef);
            Assert.AreEqual(OSDType.Unknown, llsdUndef.Type);
        }

        [Test()]
        public void SerializeUndef()
        {
            OSD llsdUndef = new OSD();
            byte[] binaryUndefSerialized = OSDParser.SerializeLLSDBinary(llsdUndef);
            Assert.AreEqual(binaryUndef, binaryUndefSerialized);
        }

        private static byte[] binaryTrueValue = { 0x31 };
        private static byte[] binaryTrue = (byte[])ConcatenateArrays(binaryHead, binaryTrueValue);


        private static byte[] binaryFalseValue = { 0x30 };
        private static byte[] binaryFalse = (byte[])ConcatenateArrays(binaryHead, binaryFalseValue);

        [Test()]
        public void DeserializeBool()
        {
            OSD llsdTrue = OSDParser.DeserializeLLSDBinary(binaryTrue);
            Assert.AreEqual(OSDType.Boolean, llsdTrue.Type);
            Assert.AreEqual(true, llsdTrue.AsBoolean());

            OSD llsdFalse = OSDParser.DeserializeLLSDBinary(binaryFalse);
            Assert.AreEqual(OSDType.Boolean, llsdFalse.Type);
            Assert.AreEqual(false, llsdFalse.AsBoolean());
        }

        [Test()]
        public void SerializeBool()
        {
            OSD llsdTrue = OSD.FromBoolean(true);
            byte[] binaryTrueSerialized = OSDParser.SerializeLLSDBinary(llsdTrue);
            Assert.AreEqual(binaryTrue, binaryTrueSerialized);

            OSD llsdFalse = OSD.FromBoolean(false);
            byte[] binaryFalseSerialized = OSDParser.SerializeLLSDBinary(llsdFalse);
            Assert.AreEqual(binaryFalse, binaryFalseSerialized);
        }

        private static byte[] binaryZeroIntValue = { 0x69, 0x0, 0x0, 0x0, 0x0 };
        private static byte[] binaryZeroInt = (byte[])ConcatenateArrays(binaryHead, binaryZeroIntValue);

        private static byte[] binaryAnIntValue = { 0x69, 0x0, 0x12, 0xd7, 0x9b };
        private static byte[] binaryAnInt = (byte[])ConcatenateArrays(binaryHead, binaryAnIntValue);

        [Test()]
        public void DeserializeInteger()
        {
            OSD llsdZeroInteger = OSDParser.DeserializeLLSDBinary(binaryZeroInt);
            Assert.AreEqual(OSDType.Integer, llsdZeroInteger.Type);
            Assert.AreEqual(0, llsdZeroInteger.AsInteger());


            OSD llsdAnInteger = OSDParser.DeserializeLLSDBinary(binaryAnInt);
            Assert.AreEqual(OSDType.Integer, llsdAnInteger.Type);
            Assert.AreEqual(1234843, llsdAnInteger.AsInteger());
        }

        [Test()]
        public void SerializeInteger()
        {
            OSD llsdZeroInt = OSD.FromInteger(0);
            byte[] binaryZeroIntSerialized = OSDParser.SerializeLLSDBinary(llsdZeroInt);
            Assert.AreEqual(binaryZeroInt, binaryZeroIntSerialized);

            OSD llsdAnInt = OSD.FromInteger(1234843);
            byte[] binaryAnIntSerialized = OSDParser.SerializeLLSDBinary(llsdAnInt);
            Assert.AreEqual(binaryAnInt, binaryAnIntSerialized);
        }

        private static byte[] binaryRealValue = { 0x72, 0x41, 0x2c, 0xec, 0xf6, 0x77, 0xce, 0xd9, 0x17 };
        private static byte[] binaryReal = (byte[])ConcatenateArrays(binaryHead, binaryRealValue);

        [Test()]
        public void DeserializeReal()
        {
            OSD llsdReal = OSDParser.DeserializeLLSDBinary(binaryReal);
            Assert.AreEqual(OSDType.Real, llsdReal.Type);
            Assert.AreEqual(947835.234d, llsdReal.AsReal());
        }

        [Test()]
        public void SerializeReal()
        {
            OSD llsdReal = OSD.FromReal(947835.234d);
            byte[] binaryRealSerialized = OSDParser.SerializeLLSDBinary(llsdReal);
            Assert.AreEqual(binaryReal, binaryRealSerialized);
        }

        private static byte[] binaryAUUIDValue = { 0x75, 0x97, 0xf4, 0xae, 0xca, 0x88, 0xa1, 0x42, 0xa1, 
                                        0xb3, 0x85, 0xb9, 0x7b, 0x18, 0xab, 0xb2, 0x55 };
        private static byte[] binaryAUUID = (byte[])ConcatenateArrays(binaryHead, binaryAUUIDValue);

        private static byte[] binaryZeroUUIDValue = { 0x75, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
        private static byte[] binaryZeroUUID = (byte[])ConcatenateArrays(binaryHead, binaryZeroUUIDValue);


        [Test()]
        public void DeserializeUUID()
        {
            OSD llsdAUUID = OSDParser.DeserializeLLSDBinary(binaryAUUID);
            Assert.AreEqual(OSDType.UUID, llsdAUUID.Type);
            Assert.AreEqual("97f4aeca-88a1-42a1-b385-b97b18abb255", llsdAUUID.AsString());

            OSD llsdZeroUUID = OSDParser.DeserializeLLSDBinary(binaryZeroUUID);
            Assert.AreEqual(OSDType.UUID, llsdZeroUUID.Type);
            Assert.AreEqual("00000000-0000-0000-0000-000000000000", llsdZeroUUID.AsString());

        }

        [Test()]
        public void SerializeUUID()
        {
            OSD llsdAUUID = OSD.FromUUID(new UUID("97f4aeca-88a1-42a1-b385-b97b18abb255"));
            byte[] binaryAUUIDSerialized = OSDParser.SerializeLLSDBinary(llsdAUUID);
            Assert.AreEqual(binaryAUUID, binaryAUUIDSerialized);

            OSD llsdZeroUUID = OSD.FromUUID(new UUID("00000000-0000-0000-0000-000000000000"));
            byte[] binaryZeroUUIDSerialized = OSDParser.SerializeLLSDBinary(llsdZeroUUID);
            Assert.AreEqual(binaryZeroUUID, binaryZeroUUIDSerialized);
        }

        private static byte[] binaryBinStringValue = { 0x62, 0x0, 0x0, 0x0, 0x34, // this line is the encoding header
                                        0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67, 0x20, 0x61, 0x20, 0x73, 
                                        0x69, 0x6d, 0x70, 0x6c, 0x65, 0x20, 0x62, 0x69, 0x6e, 0x61, 0x72, 0x79, 0x20, 0x63, 0x6f,
                                        0x6e, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6f, 0x6e, 0x20, 0x66, 0x6f, 0x72, 0x20, 0x74, 0x68,
                                        0x69, 0x73, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6e, 0x67, 0xa, 0xd };
        private static byte[] binaryBinString = (byte[])ConcatenateArrays(binaryHead, binaryBinStringValue);

        [Test()]
        public void DeserializeLLSDBinary()
        {
            OSD llsdBytes = OSDParser.DeserializeLLSDBinary(binaryBinString);
            Assert.AreEqual(OSDType.Binary, llsdBytes.Type);
            byte[] contentBinString = { 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67, 0x20, 0x61, 0x20, 0x73, 
                                        0x69, 0x6d, 0x70, 0x6c, 0x65, 0x20, 0x62, 0x69, 0x6e, 0x61, 0x72, 0x79, 0x20, 0x63, 0x6f,
                                        0x6e, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6f, 0x6e, 0x20, 0x66, 0x6f, 0x72, 0x20, 0x74, 0x68,
                                        0x69, 0x73, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6e, 0x67, 0xa, 0xd };
            Assert.AreEqual(contentBinString, llsdBytes.AsBinary());
        }

        [Test()]
        public void SerializeLLSDBinary()
        {
            byte[] contentBinString = { 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67, 0x20, 0x61, 0x20, 0x73, 
                                        0x69, 0x6d, 0x70, 0x6c, 0x65, 0x20, 0x62, 0x69, 0x6e, 0x61, 0x72, 0x79, 0x20, 0x63, 0x6f,
                                        0x6e, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6f, 0x6e, 0x20, 0x66, 0x6f, 0x72, 0x20, 0x74, 0x68,
                                        0x69, 0x73, 0x20, 0x73, 0x74, 0x72, 0x69, 0x6e, 0x67, 0xa, 0xd };
            OSD llsdBinary = OSD.FromBinary(contentBinString);
            byte[] binaryBinarySerialized = OSDParser.SerializeLLSDBinary(llsdBinary);
            Assert.AreEqual(binaryBinString, binaryBinarySerialized);
        }

        private static byte[] binaryEmptyStringValue = { 0x73, 0x0, 0x0, 0x0, 0x0 };
        private static byte[] binaryEmptyString = (byte[])ConcatenateArrays(binaryHead, binaryEmptyStringValue);
        private static byte[] binaryLongStringValue = { 0x73, 0x0, 0x0, 0x0, 0x25, 
                                                            0x61, 0x62, 0x63, 0x64, 0x65, 0x66,
                                                            0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c,
                                                            0x6d, 0x6e, 0x6f, 0x70, 0x71, 0x72,
                                                            0x73, 0x74, 0x75, 0x76, 0x77, 0x78,
                                                            0x79, 0x7a, 0x30, 0x31, 0x32, 0x33,
                                                            0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x30 };
        private static byte[] binaryLongString = (byte[])ConcatenateArrays(binaryHead, binaryLongStringValue);

        [Test()]
        public void DeserializeString()
        {
            OSD llsdEmptyString = OSDParser.DeserializeLLSDBinary(binaryEmptyString);
            Assert.AreEqual(OSDType.String, llsdEmptyString.Type);
            string contentEmptyString = "";
            Assert.AreEqual(contentEmptyString, llsdEmptyString.AsString());

            OSD llsdLongString = OSDParser.DeserializeLLSDBinary(binaryLongString);
            Assert.AreEqual(OSDType.String, llsdLongString.Type);
            string contentLongString = "abcdefghijklmnopqrstuvwxyz01234567890";
            Assert.AreEqual(contentLongString, llsdLongString.AsString());
        }

        [Test()]
        public void SerializeString()
        {
            OSD llsdString = OSD.FromString("abcdefghijklmnopqrstuvwxyz01234567890");
            byte[] binaryLongStringSerialized = OSDParser.SerializeLLSDBinary(llsdString);
            Assert.AreEqual(binaryLongString, binaryLongStringSerialized);

            // A test with some utf8 characters
            string contentAStringXML = "<x>&#x196;&#x214;&#x220;&#x228;&#x246;&#x252;</x>";
            byte[] bytes = Encoding.UTF8.GetBytes(contentAStringXML);
            XmlTextReader xtr = new XmlTextReader(new MemoryStream(bytes, false));
            xtr.Read();
            xtr.Read();

            string contentAString = xtr.ReadString();
            OSD llsdAString = OSD.FromString(contentAString);
            byte[] binaryAString = OSDParser.SerializeLLSDBinary(llsdAString);
            OSD llsdAStringDS = OSDParser.DeserializeLLSDBinary(binaryAString);
            Assert.AreEqual(OSDType.String, llsdAStringDS.Type);
            Assert.AreEqual(contentAString, llsdAStringDS.AsString());

            // we also test for a 4byte character.
            string xml = "<x>&#x10137;</x>";
            byte[] bytesTwo = Encoding.UTF8.GetBytes(xml);
            XmlTextReader xtrTwo = new XmlTextReader(new MemoryStream(bytesTwo, false));
            xtrTwo.Read();
            xtrTwo.Read();
            string content = xtrTwo.ReadString();

            OSD llsdStringOne = OSD.FromString(content);
            byte[] binaryAStringOneSerialized = OSDParser.SerializeLLSDBinary(llsdStringOne);
            OSD llsdStringOneDS = OSDParser.DeserializeLLSDBinary(binaryAStringOneSerialized);
            Assert.AreEqual(OSDType.String, llsdStringOneDS.Type);
            Assert.AreEqual(content, llsdStringOneDS.AsString());

        }

        // Be careful. The current and above mentioned reference implementation has a bug that
        // doesnt allow proper binary Uri encoding.
        // We compare here to a fixed version of Uri encoding
        private static byte[] binaryURIValue = { 0x6c, 0x0, 0x0, 0x0, 0x18, // this line is the encoding header
                                    0x68, 0x74, 0x74, 0x70, 0x3a, 0x2f, 0x2f, 0x77, 0x77, 0x77, 0x2e, 0x74,
                                    0x65, 0x73, 0x74, 0x75, 0x72, 0x6c, 0x2e, 0x74, 0x65, 0x73, 0x74, 0x2f };
        private static byte[] binaryURI = (byte[])ConcatenateArrays(binaryHead, binaryURIValue);

        [Test()]
        public void DeserializeURI()
        {
            OSD llsdURI = OSDParser.DeserializeLLSDBinary(binaryURI);
            Assert.AreEqual(OSDType.URI, llsdURI.Type);
            Uri uri = new Uri("http://www.testurl.test/");
            Assert.AreEqual(uri, llsdURI.AsUri());

        }

        [Test()]
        public void SerializeURI()
        {
            OSD llsdUri = OSD.FromUri(new Uri("http://www.testurl.test/"));
            byte[] binaryURISerialized = OSDParser.SerializeLLSDBinary(llsdUri);
            Assert.AreEqual(binaryURI, binaryURISerialized);
        }

        // Here is a problem.
        // The reference implementation does serialize to a local timestamp and not to a universal timestamp,
        // which means, this implementation and the reference implementation only work the same in the universal
        // timezone. Therefore this binaryDateTimeValue is generated in the UTC timezone by the reference
        // implementation.
        private static byte[] binaryDateTimeValue = { 0x64, 0x41, 0xd1, 0xde, 0xa7, 0x8d, 0xc0, 0x0, 0x0 };
        private static byte[] binaryDateTime = (byte[])ConcatenateArrays(binaryHead, binaryDateTimeValue);

        [Test()]
        public void DeserializeDateTime()
        {
            OSD llsdDateTime = OSDParser.DeserializeLLSDBinary(binaryDateTime);
            Assert.AreEqual(OSDType.Date, llsdDateTime.Type);
            DateTime dt = new DateTime(2008, 1, 1, 20, 10, 31, 0, DateTimeKind.Utc);
            DateTime dateLocal = llsdDateTime.AsDate();
            Assert.AreEqual(dt, dateLocal.ToUniversalTime());
        }

        [Test()]
        public void SerializeDateTime()
        {
            DateTime dt = new DateTime(2008, 1, 1, 20, 10, 31, 0, DateTimeKind.Utc);
            OSD llsdDate = OSD.FromDate(dt);
            byte[] binaryDateSerialized = OSDParser.SerializeLLSDBinary(llsdDate);
            Assert.AreEqual(binaryDateTime, binaryDateSerialized);

            // check if a *local* time can be serialized and deserialized
            DateTime dtOne = new DateTime(2009, 12, 30, 8, 25, 10, DateTimeKind.Local);
            OSD llsdDateOne = OSD.FromDate(dtOne);
            byte[] binaryDateOneSerialized = OSDParser.SerializeLLSDBinary(llsdDateOne);
            OSD llsdDateOneDS = OSDParser.DeserializeLLSDBinary(binaryDateOneSerialized);
            Assert.AreEqual(OSDType.Date, llsdDateOneDS.Type);
            Assert.AreEqual(dtOne, llsdDateOneDS.AsDate());

            DateTime dtTwo = new DateTime(2010, 11, 11, 10, 8, 20, DateTimeKind.Utc);
            OSD llsdDateTwo = OSD.FromDate(dtTwo);
            byte[] binaryDateTwoSerialized = OSDParser.SerializeLLSDBinary(llsdDateTwo);
            OSD llsdDateTwoDS = OSDParser.DeserializeLLSDBinary(binaryDateTwoSerialized);
            Assert.AreEqual(OSDType.Date, llsdDateOneDS.Type);
            Assert.AreEqual(dtTwo.ToLocalTime(), llsdDateTwoDS.AsDate());

        }

        // Data for empty array { }
        private static byte[] binaryEmptyArrayValue = { 0x5b, 0x0, 0x0, 0x0, 0x0, 0x5d };
        // Encoding header + num of elements + tail
        private static byte[] binaryEmptyArray = (byte[])ConcatenateArrays(binaryHead, binaryEmptyArrayValue);
        // Data for simple array { 0 }
        private static byte[] binarySimpleArrayValue = { 0x5b, 0x0, 0x0, 0x0, 0x1, // Encoding header + num of elements
                                                             0x69, 0x0, 0x0, 0x0, 0x0, 0x5d };
        private static byte[] binarySimpleArray = (byte[])ConcatenateArrays(binaryHead, binarySimpleArrayValue);

        // Data for simple array { 0, 0 }
        private static byte[] binarySimpleArrayTwoValue = { 0x5b, 0x0, 0x0, 0x0, 0x2, // Encoding header + num of elements
                                                             0x69, 0x0, 0x0, 0x0, 0x0, 
                                                             0x69, 0x0, 0x0, 0x0, 0x0, 0x5d };
        private static byte[] binarySimpleArrayTwo = (byte[])ConcatenateArrays(binaryHead, binarySimpleArrayTwoValue);

        [Test()]
        public void DeserializeArray()
        {
            OSD llsdEmptyArray = OSDParser.DeserializeLLSDBinary(binaryEmptyArray);
            Assert.AreEqual(OSDType.Array, llsdEmptyArray.Type);
            OSDArray llsdEmptyArrayArray = (OSDArray)llsdEmptyArray;
            Assert.AreEqual(0, llsdEmptyArrayArray.Count);


            OSD llsdSimpleArray = OSDParser.DeserializeLLSDBinary(binarySimpleArray);
            Assert.AreEqual(OSDType.Array, llsdSimpleArray.Type);
            OSDArray llsdArray = (OSDArray)llsdSimpleArray;
            Assert.AreEqual(OSDType.Integer, llsdArray[0].Type);
            Assert.AreEqual(0, llsdArray[0].AsInteger());


            OSD llsdSimpleArrayTwo = OSDParser.DeserializeLLSDBinary(binarySimpleArrayTwo);
            Assert.AreEqual(OSDType.Array, llsdSimpleArrayTwo.Type);
            OSDArray llsdArrayTwo = (OSDArray)llsdSimpleArrayTwo;
            Assert.AreEqual(2, llsdArrayTwo.Count);

            Assert.AreEqual(OSDType.Integer, llsdArrayTwo[0].Type);
            Assert.AreEqual(0, llsdArrayTwo[0].AsInteger());
            Assert.AreEqual(OSDType.Integer, llsdArrayTwo[1].Type);
            Assert.AreEqual(0, llsdArrayTwo[1].AsInteger());
        }

        [Test()]
        public void SerializeArray()
        {
            OSDArray llsdEmptyArray = new OSDArray();
            byte[] binaryEmptyArraySerialized = OSDParser.SerializeLLSDBinary(llsdEmptyArray);
            Assert.AreEqual(binaryEmptyArray, binaryEmptyArraySerialized);

            OSDArray llsdSimpleArray = new OSDArray();
            llsdSimpleArray.Add(OSD.FromInteger(0));
            byte[] binarySimpleArraySerialized = OSDParser.SerializeLLSDBinary(llsdSimpleArray);
            Assert.AreEqual(binarySimpleArray, binarySimpleArraySerialized);

            OSDArray llsdSimpleArrayTwo = new OSDArray();
            llsdSimpleArrayTwo.Add(OSD.FromInteger(0));
            llsdSimpleArrayTwo.Add(OSD.FromInteger(0));
            byte[] binarySimpleArrayTwoSerialized = OSDParser.SerializeLLSDBinary(llsdSimpleArrayTwo);
            Assert.AreEqual(binarySimpleArrayTwo, binarySimpleArrayTwoSerialized);

        }

        // Data for empty dictionary { }
        private static byte[] binaryEmptyMapValue = { 0x7b, 0x0, 0x0, 0x0, 0x0, 0x7d };
        private static byte[] binaryEmptyMap = (byte[])ConcatenateArrays(binaryHead, binaryEmptyMapValue);

        // Data for simple dictionary { test = 0 }
        private static byte[] binarySimpleMapValue = { 0x7b, 0x0, 0x0, 0x0, 0x1, // Encoding header + num of elements
                                                        0x6b, 0x0, 0x0, 0x0, 0x4, // 'k' + keylength 
                                                        0x74, 0x65, 0x73, 0x74,  // key 'test' 
                                                        0x69, 0x0, 0x0, 0x0, 0x0, // i + '0'
                                                        0x7d };
        private static byte[] binarySimpleMap = (byte[])ConcatenateArrays(binaryHead, binarySimpleMapValue);

        // Data for simple dictionary { t0st = 241, tes1 = "aha", test = undef }
        private static byte[] binarySimpleMapTwoValue = { 0x7b, 0x0, 0x0, 0x0, 0x3, // Encoding header + num of elements
                                 0x6b, 0x0, 0x0, 0x0, 0x4, // 'k' + keylength 
                                 0x74, 0x65, 0x73, 0x74,  // key 'test'
                                 0x21, // undef
                                 0x6b, 0x0, 0x0, 0x0, 0x4, // k + keylength 
                                 0x74, 0x65, 0x73, 0x31, // key 'tes1' 
                                 0x73, 0x0, 0x0, 0x0, 0x3, // string head + length
                                 0x61, 0x68, 0x61, // 'aha' 
                                 0x6b, 0x0, 0x0, 0x0, 0x4, // k + keylength 
                                 0x74, 0x30, 0x73, 0x74,  // key 't0st'
                                 0x69, 0x0, 0x0, 0x0, 0xf1, // integer 241
                                 0x7d };
        private static byte[] binarySimpleMapTwo = (byte[])ConcatenateArrays(binaryHead, binarySimpleMapTwoValue);

        [Test()]
        public void DeserializeDictionary()
        {
            OSDMap llsdEmptyMap = (OSDMap)OSDParser.DeserializeLLSDBinary(binaryEmptyMap);
            Assert.AreEqual(OSDType.Map, llsdEmptyMap.Type);
            Assert.AreEqual(0, llsdEmptyMap.Count);

            OSDMap llsdSimpleMap = (OSDMap)OSDParser.DeserializeLLSDBinary(binarySimpleMap);
            Assert.AreEqual(OSDType.Map, llsdSimpleMap.Type);
            Assert.AreEqual(1, llsdSimpleMap.Count);
            Assert.AreEqual(OSDType.Integer, llsdSimpleMap["test"].Type);
            Assert.AreEqual(0, llsdSimpleMap["test"].AsInteger());

            OSDMap llsdSimpleMapTwo = (OSDMap)OSDParser.DeserializeLLSDBinary(binarySimpleMapTwo);
            Assert.AreEqual(OSDType.Map, llsdSimpleMapTwo.Type);
            Assert.AreEqual(3, llsdSimpleMapTwo.Count);
            Assert.AreEqual(OSDType.Unknown, llsdSimpleMapTwo["test"].Type);
            Assert.AreEqual(OSDType.String, llsdSimpleMapTwo["tes1"].Type);
            Assert.AreEqual("aha", llsdSimpleMapTwo["tes1"].AsString());
            Assert.AreEqual(OSDType.Integer, llsdSimpleMapTwo["t0st"].Type);
            Assert.AreEqual(241, llsdSimpleMapTwo["t0st"].AsInteger());


        }

        [Test()]
        public void SerializeDictionary()
        {
            OSDMap llsdEmptyMap = new OSDMap();
            byte[] binaryEmptyMapSerialized = OSDParser.SerializeLLSDBinary(llsdEmptyMap);
            Assert.AreEqual(binaryEmptyMap, binaryEmptyMapSerialized);

            OSDMap llsdSimpleMap = new OSDMap();
            llsdSimpleMap["test"] = OSD.FromInteger(0);
            byte[] binarySimpleMapSerialized = OSDParser.SerializeLLSDBinary(llsdSimpleMap);
            Assert.AreEqual(binarySimpleMap, binarySimpleMapSerialized);

            OSDMap llsdSimpleMapTwo = new OSDMap();
            llsdSimpleMapTwo["t0st"] = OSD.FromInteger(241);
            llsdSimpleMapTwo["tes1"] = OSD.FromString("aha");
            llsdSimpleMapTwo["test"] = new OSD();
            byte[] binarySimpleMapTwoSerialized = OSDParser.SerializeLLSDBinary(llsdSimpleMapTwo);

            // We dont compare here to the original serialized value, because, as maps dont preserve order,
            // the original serialized value is not *exactly* the same. Instead we compare to a deserialized
            // version created by this deserializer.
            OSDMap llsdSimpleMapDeserialized = (OSDMap)OSDParser.DeserializeLLSDBinary(binarySimpleMapTwoSerialized);
            Assert.AreEqual(OSDType.Map, llsdSimpleMapDeserialized.Type);
            Assert.AreEqual(3, llsdSimpleMapDeserialized.Count);
            Assert.AreEqual(OSDType.Integer, llsdSimpleMapDeserialized["t0st"].Type);
            Assert.AreEqual(241, llsdSimpleMapDeserialized["t0st"].AsInteger());
            Assert.AreEqual(OSDType.String, llsdSimpleMapDeserialized["tes1"].Type);
            Assert.AreEqual("aha", llsdSimpleMapDeserialized["tes1"].AsString());
            Assert.AreEqual(OSDType.Unknown, llsdSimpleMapDeserialized["test"].Type);

            // we also test for a 4byte key character.
            string xml = "<x>&#x10137;</x>";
            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            XmlTextReader xtr = new XmlTextReader(new MemoryStream(bytes, false));
            xtr.Read();
            xtr.Read();
            string content = xtr.ReadString();

            OSDMap llsdSimpleMapThree = new OSDMap();
            OSD llsdSimpleValue = OSD.FromString(content);
            llsdSimpleMapThree[content] = llsdSimpleValue;
            Assert.AreEqual(content, llsdSimpleMapThree[content].AsString());

            byte[] binarySimpleMapThree = OSDParser.SerializeLLSDBinary(llsdSimpleMapThree);
            OSDMap llsdSimpleMapThreeDS = (OSDMap)OSDParser.DeserializeLLSDBinary(binarySimpleMapThree);
            Assert.AreEqual(OSDType.Map, llsdSimpleMapThreeDS.Type);
            Assert.AreEqual(1, llsdSimpleMapThreeDS.Count);
            Assert.AreEqual(content, llsdSimpleMapThreeDS[content].AsString());

        }

        private static byte[] binaryNestedValue = { 0x5b, 0x0, 0x0, 0x0, 0x3, 
                                            0x7b, 0x0, 0x0, 0x0, 0x2, 
                                            0x6b, 0x0, 0x0, 0x0, 0x4, 
                                            0x74, 0x65, 0x73, 0x74, 
                                            0x73, 0x0, 0x0, 0x0, 0x4, 
                                            0x77, 0x68, 0x61, 0x74, 
                                            0x6b, 0x0, 0x0, 0x0, 0x4, 
                                            0x74, 0x30, 0x73, 
                                            0x74, 0x5b, 0x0, 0x0, 0x0, 0x2,
                                            0x69, 0x0, 0x0, 0x0, 0x1,
                                            0x69, 0x0, 0x0, 0x0, 0x2,
                                            0x5d, 0x7d, 0x69, 0x0, 0x0, 0x0, 
                                            0x7c, 0x69, 0x0, 0x0, 0x3, 0xdb, 
                                           0x5d };
        private static byte[] binaryNested = (byte[])ConcatenateArrays(binaryHead, binaryNestedValue);

        [Test()]
        public void DeserializeNestedComposite()
        {
            OSD llsdNested = OSDParser.DeserializeLLSDBinary(binaryNested);
            Assert.AreEqual(OSDType.Array, llsdNested.Type);
            OSDArray llsdArray = (OSDArray)llsdNested;
            Assert.AreEqual(3, llsdArray.Count);

            OSDMap llsdMap = (OSDMap)llsdArray[0];
            Assert.AreEqual(OSDType.Map, llsdMap.Type);
            Assert.AreEqual(2, llsdMap.Count);

            OSDArray llsdNestedArray = (OSDArray)llsdMap["t0st"];
            Assert.AreEqual(OSDType.Array, llsdNestedArray.Type);
            OSDInteger llsdNestedIntOne = (OSDInteger)llsdNestedArray[0];
            Assert.AreEqual(OSDType.Integer, llsdNestedIntOne.Type);
            Assert.AreEqual(1, llsdNestedIntOne.AsInteger());
            OSDInteger llsdNestedIntTwo = (OSDInteger)llsdNestedArray[1];
            Assert.AreEqual(OSDType.Integer, llsdNestedIntTwo.Type);
            Assert.AreEqual(2, llsdNestedIntTwo.AsInteger());

            OSDString llsdString = (OSDString)llsdMap["test"];
            Assert.AreEqual(OSDType.String, llsdString.Type);
            Assert.AreEqual("what", llsdString.AsString());

            OSDInteger llsdIntOne = (OSDInteger)llsdArray[1];
            Assert.AreEqual(OSDType.Integer, llsdIntOne.Type);
            Assert.AreEqual(124, llsdIntOne.AsInteger());
            OSDInteger llsdIntTwo = (OSDInteger)llsdArray[2];
            Assert.AreEqual(OSDType.Integer, llsdIntTwo.Type);
            Assert.AreEqual(987, llsdIntTwo.AsInteger());

        }

        [Test()]
        public void SerializeNestedComposite()
        {
            OSDArray llsdNested = new OSDArray();
            OSDMap llsdMap = new OSDMap();
            OSDArray llsdArray = new OSDArray();
            llsdArray.Add(OSD.FromInteger(1));
            llsdArray.Add(OSD.FromInteger(2));
            llsdMap["t0st"] = llsdArray;
            llsdMap["test"] = OSD.FromString("what");
            llsdNested.Add(llsdMap);
            llsdNested.Add(OSD.FromInteger(124));
            llsdNested.Add(OSD.FromInteger(987));

            byte[] binaryNestedSerialized = OSDParser.SerializeLLSDBinary(llsdNested);
            // Because maps don't preserve order, we compare here to a deserialized value. 
            OSDArray llsdNestedDeserialized = (OSDArray)OSDParser.DeserializeLLSDBinary(binaryNestedSerialized);
            Assert.AreEqual(OSDType.Array, llsdNestedDeserialized.Type);
            Assert.AreEqual(3, llsdNestedDeserialized.Count);

            OSDMap llsdMapDeserialized = (OSDMap)llsdNestedDeserialized[0];
            Assert.AreEqual(OSDType.Map, llsdMapDeserialized.Type);
            Assert.AreEqual(2, llsdMapDeserialized.Count);
            Assert.AreEqual(OSDType.Array, llsdMapDeserialized["t0st"].Type);

            OSDArray llsdNestedArray = (OSDArray)llsdMapDeserialized["t0st"];
            Assert.AreEqual(OSDType.Array, llsdNestedArray.Type);
            Assert.AreEqual(2, llsdNestedArray.Count);
            Assert.AreEqual(OSDType.Integer, llsdNestedArray[0].Type);
            Assert.AreEqual(1, llsdNestedArray[0].AsInteger());
            Assert.AreEqual(OSDType.Integer, llsdNestedArray[1].Type);
            Assert.AreEqual(2, llsdNestedArray[1].AsInteger());

            Assert.AreEqual(OSDType.String, llsdMapDeserialized["test"].Type);
            Assert.AreEqual("what", llsdMapDeserialized["test"].AsString());

            Assert.AreEqual(OSDType.Integer, llsdNestedDeserialized[1].Type);
            Assert.AreEqual(124, llsdNestedDeserialized[1].AsInteger());

            Assert.AreEqual(OSDType.Integer, llsdNestedDeserialized[2].Type);
            Assert.AreEqual(987, llsdNestedDeserialized[2].AsInteger());

        }

        [Test()]
        public void SerializeLongMessage()
        {
            // each 80 chars
            string sOne = "asdklfjasadlfkjaerotiudfgjkhsdklgjhsdklfghasdfklhjasdfkjhasdfkljahsdfjklaasdfkj8";
            string sTwo = "asdfkjlaaweoiugsdfjkhsdfg,.mnasdgfkljhrtuiohfgl�kajsdfoiwghjkdlaaaaseldkfjgheus9";
            
            OSD stringOne = OSD.FromString( sOne );
            OSD stringTwo = OSD.FromString(sTwo);

            OSDMap llsdMap = new OSDMap();
            llsdMap["testOne"] = stringOne;
            llsdMap["testTwo"] = stringTwo;
            llsdMap["testThree"] = stringOne;
            llsdMap["testFour"] = stringTwo;
            llsdMap["testFive"] = stringOne;
            llsdMap["testSix"] = stringTwo;
            llsdMap["testSeven"] = stringOne;
            llsdMap["testEight"] = stringTwo;
            llsdMap["testNine"] = stringOne;
            llsdMap["testTen"] = stringTwo;
            
            
            byte[] binaryData = OSDParser.SerializeLLSDBinary( llsdMap );

            OSDMap llsdMapDS = (OSDMap)OSDParser.DeserializeLLSDBinary( binaryData );
            Assert.AreEqual( OSDType.Map, llsdMapDS.Type );
            Assert.AreEqual( 10, llsdMapDS.Count );
            Assert.AreEqual( sOne, llsdMapDS["testOne"].AsString());
            Assert.AreEqual( sTwo, llsdMapDS["testTwo"].AsString());
            Assert.AreEqual( sOne, llsdMapDS["testThree"].AsString());
            Assert.AreEqual( sTwo, llsdMapDS["testFour"].AsString());
            Assert.AreEqual( sOne, llsdMapDS["testFive"].AsString());
            Assert.AreEqual( sTwo, llsdMapDS["testSix"].AsString());
            Assert.AreEqual( sOne, llsdMapDS["testSeven"].AsString());
            Assert.AreEqual( sTwo, llsdMapDS["testEight"].AsString());
            Assert.AreEqual( sOne, llsdMapDS["testNine"].AsString());
            Assert.AreEqual( sTwo, llsdMapDS["testTen"].AsString());
        }


        static Array ConcatenateArrays(params Array[] arrays)
        {
            if (arrays == null)
            {
                throw new ArgumentNullException("arrays");
            }
            if (arrays.Length == 0)
            {
                throw new ArgumentException("No arrays specified");
            }

            Type type = arrays[0].GetType().GetElementType();
            int totalLength = arrays[0].Length;
            for (int i = 1; i < arrays.Length; i++)
            {
                if (arrays[i].GetType().GetElementType() != type)
                {
                    throw new ArgumentException("Arrays must all be of the same type");
                }
                totalLength += arrays[i].Length;
            }

            Array ret = Array.CreateInstance(type, totalLength);
            int index = 0;
            foreach (Array array in arrays)
            {
                Array.Copy(array, 0, ret, index, array.Length);
                index += array.Length;
            }
            return ret;
        }
    }
}

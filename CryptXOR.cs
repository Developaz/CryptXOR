using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Assertions;

namespace CryptXOR
{
    // 암복화용 변수
    [System.Serializable]
    public class CryptValue<T>
    {
        [SerializeField] private string m_value;
        public T value { get { return Crypto.Instance.Decrypt<T>( m_value ); } set { m_value = Crypto.Instance.Encrypt( value.ToString() ); } }

        public CryptValue( T _value )
        {
            value = _value;
        }
    }

    /// <summary>
    /// 랜덤Key + 랜덤Value + XOR을 이용한 암복화
    /// </summary>
    public class Crypto
    {
        private List<byte[]> keys;
        private const byte NO_VALUE_INDEX = 255;

        private static IHCrypto m_instance;
        public static IHCrypto Instance
        {
            get
            {
                if( m_instance == null )
                {
                    m_instance = new IHCrypto();
                }

                return m_instance;
            }
        }

        public Crypto()
        {
            // 암호화용 키 설정
            string[] keyTable = {
                "ec1d85570adc46e1a50ba492eddf2415", "a02e9b2a8a7e49f99901a4cdb9f53ad8", "01b95d1a2c5f4077865a223366e525ce", "f93410c6799c40b0b508694b10fe1ff7",
                "ff7a3e4a8f054f8fb380a88e2d029855", "3f8adfd3ae8b467d96546230116fd3d6", "b603875e5e3c47dd814abb51e6ff508d", "76502ecca3f1475fa1c5dfdaa3f04b28",
                "7fe509bc14274bc7b1256581787dcaef", "fb4038e0c1204c29bf990890eaada63f", "f571fe6f6fa046a79cd2fdb947181c04", "54679b81824245f7ab5714db2a28d788",
                "7ca4fe1d10f647a98f189f265fd21525", "28abb05ff9e64b2ea6a4f800907d33b2", "4ec1b964e2cc44c48e3bb303ff2bba7d", "2d6d85fe21824d31bc358bfd855435e0",
                "9b6a52e973474226aae8616195657149", "6d0b3ff596f34086905deed994ea0cc6", "5792bb739cfa41fb97492322e5249c57", "f13fb2147a1447a084d4116f0e7986c3",
                "abb25a1923244c138827bdedb9083f1b", "763eff2a081e412d9c0b837ff5e67ec9", "30efdda2dffd4e3092fe4c063b6c0751", "8e8e7ff5558849e7a144625c15c9c95f",
                "cc0835011d6f447ebaaf1b088007c7a3", "a5999c4d99e64fec9c62b021139836c8", "6fa6fe2df378425a9cd7ca0f345c09a4", "df86398463e24aef801292ce1545e5f4",
                "63eb0b4693c4400c9c3a3158d139b755", "fe65740d77ff4a54993d50eeb393b04d", "09ae7d9dc9734cd787cd7a98fb2a8845", "0d16c30be8874aecb6a83dd3adb6fac4",
            };

            byte count = (byte)keyTable.Length;
            keys = new List<byte[]>();

            for( int i = 0; i < count; i++ )
            {
                keys.Add( Encoding.UTF8.GetBytes( keyTable[ i ] ) );
            }
        }

        private string encrypt( string _value )
        {
            System.Random rand = new System.Random();

            byte keyIndex = (byte)( rand.Next( 0, 100000 ) % keys.Count );

            byte[] v = Encoding.UTF8.GetBytes( _value.ToString() );
            byte[] k = keys[ keyIndex ];

            // _value의 길이가 10보다 작을경우 더미값을 채워서 암호화
            byte addValue = v.Length < 10 ? (byte)( 10 - v.Length ) : (byte)0;
            byte[] r = new byte[ v.Length + 2 + addValue ];

            r[ 0 ] = keyIndex;
            r[ 1 ] = addValue == 0 ? NO_VALUE_INDEX : addValue;

            for( int i = v.Length + 2; i < r.Length; i++ )
            {
                r[ i ] = (byte)rand.Next( 0, 100000 );
            }

            for( int i = 0; i < v.Length; i++ )
            {
                r[ i + 2 ] = (byte)( v[ i ] ^ k[ ( i % k.Length ) ] );
            }

            return Convert.ToBase64String( r );
        }

        private string decrypt( string _value )
        {
            byte[] v = Convert.FromBase64String( _value );
            byte keyIndex = v[ 0 ];
            byte addValue = v[ 1 ] == NO_VALUE_INDEX ? (byte) 0 : v[ 1 ];

            byte[] k = keys[ keyIndex ];
            byte[] r = new byte[ v.Length - 2 - addValue ];

            for( int i = 0; i < r.Length; i++ )
            {
                r[ i ] = (byte)( v[ i + 2 ] ^ k[ ( i % k.Length ) ] );
            }

            return Encoding.UTF8.GetString( r );
        }

        public string Encrypt( string _value )
        {
            return encrypt( _value.ToString() );
        }

        public T Decrypt<T>( string _value )
        {
            return (T)Convert.ChangeType( decrypt( _value ), typeof( T ) );
        }
    }
}

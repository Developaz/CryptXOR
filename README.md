# CryptXOR
AES의 암복화는 변수를 암복화 하기에는 느리기 때문에 XOR을 이용한 변수 암복화 제작
## Description
- 첫번째 byte에 key index 설정
- 두번째 byte에 암호화할 값( v )의 길이가 10보다 작을 경우 더미값을 얼만큼 채웠는지 설정
- 더미값은 값의 뒤에서 부터 채운다.
## How to Use
- keyTable에 key n개를 설정
- Public 함수인 Encrypt / Decrypt 로 암복화

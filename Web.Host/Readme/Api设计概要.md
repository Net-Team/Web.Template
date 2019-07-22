## 1 ����
> [service]
* ��[*��ϵͳ]-[��ģ��]-[��ģ��]��ɣ�����ģ���ǷǱ����
* ��ȫСд
 
> [controller]
* ����������һ�����ĳ��С���ܣ�����[books] [users] [roomstucts]�ȣ�
* �����Ǹ�����ʽ
* ��ȫСд

> [id]
* ��ʾĳ���һ��Ψһ��ʶ
* �����û������û�id

## 2 ·�ɹ���
### 2.1 �ڲ��ӿ�
> internal/[service]/[controller]/[id]

* internal/rke-admin/users
* internal/rke-admin/books/id001


### 2.2 ����ӿ�
> api/[service]/[controller]/[id]

* api/rke-admin/users
* api/rke-admin/users/id002
* api/rke-app/roomstructs
* api/rke-device/�ſڻ�/roomstructs
* api/rke-device/���ڻ�/roomstructs/id003
* api/rke-device-log/�ſڻ�/unlockedlogs
* api/rke-����/users


## 3 ����ʽ
> GET

���ڻ�ȡ��Դ������������ص��Ƕ�λ�ȡһ����Դ�������Դ�иı�ķ��գ�
�ͻ��˿�����һ��ҵ����������ж������GET������

> POST

��ͬ��add�����ڸ���Դ�����������Դ

> PUT

��ͬ�ڸ�����update���������������Դ���ݸ��Ǹ��µ����еļ�¼��

> PATCH

��ͬ��ѡ����update�����������Ȥ��һ���������Ը��µ����м�¼��Ӧ���ֶ���

> DELETE

ɾ����;�����ڵ�һ��¼��·��һ�����ΪDELETE api/[service]/[controller]/[id]

## 4 ��ӦԼ��
### 4.1 http״̬���������쳣
* 401 δ��Ȩ������
* 404 ����api������
* 405 ����ȷ������ʽ
* 500 �������ڲ��쳣

### 4.2 ʹ��ͳһ��ģ������ҵ��״̬������
* code����ҵ�����״̬��
* messageΪ��Ӧ����ʾ��
* dataΪ�ɹ�ʱ��Ӧ�Ľӿ�����ģ��
* http״̬��Ϊ200��299

```
{
  "code": 0,
  "message": "�ɹ�",
  "data": {
    "name": "laojiu",
    "age": 99     
  }
}
```


## 5 ��ѯ�������ҳģ��

### 5.1 ��ѯ����
> ps �˹���δȷ���ã����պ�ǰ��ȷ����ʹ��query����cookie

ǰ�˶����ѯ����ʹ��query�������ݵ���Ӧ��api

* api/rke-admin/users?name=laojiu
* api/rke-admin/users?name=laojiu&nickName=��
* api/rke-admin/users?name=laojiu&nickName=��&userType=3

���Ӧ����������Ӧǰ�˴��벻ȷ�������Ĳ�ѯ�ֶε�������
������Ȩ��ʹ�ú��ʵıȽϷ��źͺ��Դ����query��������
�������ϲ�ѯ��˿��Խ���Ϊ
* name like '%laojiu%'
* name like '%laojiu%' // ǿ�ƺ�����nickName
* name like '%laojiu%' and nickName like '%��%' and userType = 3

### 5.2 ��ҳģ��
ǰ��ʹ��pageIndex��pageSize����query������λ��ҳ��pageIndexΪ��������0��ʼ��
��˷��������ݵ�data��Ӧ���·�ҳģ��:

```
{
  "pageIndex": 0,
  "pageSize": 10,
  "totalCount": 999,
  "dataArray": [
    {},
    {}
  ]
}
```
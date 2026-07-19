# GeoPortal-Backend


## OrderManagement AuditLog entegrasyonu

OrderManagement mikroservisi de `AuditLog.Shared` middleware yapısına bağlanmıştır.
Kayıtlar kimlik doğrulamadan sonra oluşturulur; kullanıcı, rol, endpoint, işlem türü,
başarı durumu, süre, IP ve correlation id merkezi AuditLogManagement servisine gönderilir.
`/files` statik dosya yolu gereksiz kayıt üretmemesi için hariç tutulmuştur.

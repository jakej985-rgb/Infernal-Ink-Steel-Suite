#ifndef TABLECLIENT_H
#define TABLECLIENT_H

#include <QString>
#include <QVariant>

struct TableClient {
    QString id;
    QString first;
    QString middle;
    QString last;
    QString phone;
    QString email;
    QString visits;

    TableClient(
        const QString &id_ = "",
        const QString &first_ = "",
        const QString &middle_ = "",
        const QString &last_ = "",
        const QString &phone_ = "",
        const QString &email_ = "",
        const QString &visits_ = "")
        : id(id_), first(first_), middle(middle_), last(last_),
          phone(phone_), email(email_), visits(visits_) {}

    QVariant toVariant() const {
        QVariantMap map;
        map["id"] = id;
        map["first"] = first;
        map["middle"] = middle;
        map["last"] = last;
        map["phone"] = phone;
        map["email"] = email;
        map["visits"] = visits;
        return map;
    }

    static TableClient fromVariant(const QVariant &var) {
        TableClient c;
        QVariantMap map = var.toMap();
        c.id = map.value("id").toString();
        c.first = map.value("first").toString();
        c.middle = map.value("middle").toString();
        c.last = map.value("last").toString();
        c.phone = map.value("phone").toString();
        c.email = map.value("email").toString();
        c.visits = map.value("visits").toString();
        return c;
    }
};

#endif // TABLECLIENT_H
#ifndef CLIENT_H
#define CLIENT_H

#include <QString>
#include <optional>
#include <QSqlDatabase>
#include <QSqlQuery>
#include <QSqlError>
#include <QDebug>

class Client {
public:
    int id = -1;
    QString firstName;
    QString middleName;
    QString lastName;
    QString phone;
    QString email;

    Client() = default;
    Client(const QString &fn, const QString &mn, const QString &ln,
           const QString &ph, const QString &em)
        : firstName(fn), middleName(mn), lastName(ln), phone(ph), email(em) {}

    QString fullName() const {
        return firstName + (middleName.isEmpty() ? " " : " " + middleName + " ") + lastName;
    }

    static std::optional<Client> getById(const QSqlDatabase &db, int clientId) {
        QSqlQuery query(db);
        query.prepare("SELECT * FROM clients WHERE id=:id");
        query.bindValue(":id", clientId);

        if (query.exec() && query.next()) {
            Client c;
            c.id = query.value("id").toInt();
            c.firstName = query.value("firstName").toString();
            c.middleName = query.value("middleName").toString();
            c.lastName = query.value("lastName").toString();
            c.phone = query.value("phone").toString();
            c.email = query.value("email").toString();
            return c;
        }
        return std::nullopt;
    }

    bool insert(const QSqlDatabase &db) {
        QSqlQuery query(db);
        query.prepare("INSERT INTO clients (firstName, middleName, lastName, phone, email) "
                      "VALUES (:fn, :mn, :ln, :ph, :em)");
        query.bindValue(":fn", firstName);
        query.bindValue(":mn", middleName);
        query.bindValue(":ln", lastName);
        query.bindValue(":ph", phone);
        query.bindValue(":em", email);

        if (query.exec()) {
            id = query.lastInsertId().toInt();
            return true;
        } else {
            qDebug() << "Client insert failed:" << query.lastError().text();
            return false;
        }
    }

    bool update(const QSqlDatabase &db) {
        if (id == -1) return false;

        QSqlQuery query(db);
        query.prepare("UPDATE clients SET firstName=:fn, middleName=:mn, lastName=:ln, phone=:ph, email=:em WHERE id=:id");
        query.bindValue(":fn", firstName);
        query.bindValue(":mn", middleName);
        query.bindValue(":ln", lastName);
        query.bindValue(":ph", phone);
        query.bindValue(":em", email);
        query.bindValue(":id", id);

        if (!query.exec()) {
            qDebug() << "Client update failed:" << query.lastError().text();
            return false;
        }
        return true;
    }

    bool remove(const QSqlDatabase &db) {
        if (id == -1) return false;

        QSqlQuery query(db);
        query.prepare("DELETE FROM clients WHERE id=:id");
        query.bindValue(":id", id);
        if (!query.exec()) {
            qDebug() << "Client delete failed:" << query.lastError().text();
            return false;
        }
        return true;
    }
};

#endif // CLIENT_H
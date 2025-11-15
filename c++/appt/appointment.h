#ifndef APPOINTMENT_H
#define APPOINTMENT_H

#include <QString>
#include <QDateTime>
#include <QColor>
#include <QSqlDatabase>
#include <QSqlQuery>
#include <QSqlError>
#include <QDebug>
#include <optional>

class Appointment {
public:
    int id = 0;
    int clientId = 0;
    int userId = 0;
    QDateTime dateTime;
    int durationMinutes = 30;
    QString serviceType;
    QString serviceCategory;   // ✅ Added
    QString priceType;         // ✅ Added
    double priceCharged = 0;   // ✅ Added
    QString notes;
    QString clientName;
    QColor color = Qt::yellow;
    QString status = "Scheduled";

    Appointment() = default;

    QDate date() const { return dateTime.date(); }
    int duration() const { return durationMinutes; }
    QTime time() const { return dateTime.time(); }
    void setDate(const QDate &d) { dateTime.setDate(d); }
    void setTime(const QTime &t) { dateTime.setTime(t); }

    // --- CRUD Helpers ---
    static std::optional<Appointment> getById(const QSqlDatabase &db, int apptId) {
        QSqlQuery query(db);
        query.prepare("SELECT * FROM appointments WHERE id=:id");
        query.bindValue(":id", apptId);

        if (query.exec() && query.next()) {
            Appointment a;
            a.id = query.value("id").toInt();
            a.clientId = query.value("clientId").toInt();
            a.userId = query.value("userId").toInt();
            a.dateTime = QDateTime::fromString(query.value("dateTime").toString(), Qt::ISODate);
            a.durationMinutes = query.value("durationMinutes").toInt();
            a.serviceType = query.value("serviceType").toString();
            a.serviceCategory = query.value("serviceCategory").toString();  // ✅
            a.priceType = query.value("priceType").toString();              // ✅
            a.priceCharged = query.value("priceCharged").toDouble();        // ✅
            a.notes = query.value("notes").toString();
            a.clientName = query.value("clientName").toString();
            a.color = QColor(query.value("color").toString());
            a.status = query.value("status").toString();
            return a;
        }
        return std::nullopt;
    }

    bool insert(const QSqlDatabase &db) {
        QSqlQuery query(db);
        query.prepare("INSERT INTO appointments "
                      "(clientId, userId, dateTime, durationMinutes, serviceType, serviceCategory, priceType, priceCharged, notes, clientName, color, status) "
                      "VALUES (:cid, :uid, :dt, :dur, :stype, :scat, :ptype, :pcharged, :notes, :cname, :color, :status)");
        query.bindValue(":cid", clientId);
        query.bindValue(":uid", userId);
        query.bindValue(":dt", dateTime.toString(Qt::ISODate));
        query.bindValue(":dur", durationMinutes);
        query.bindValue(":stype", serviceType);
        query.bindValue(":scat", serviceCategory);   // ✅
        query.bindValue(":ptype", priceType);         // ✅
        query.bindValue(":pcharged", priceCharged);   // ✅
        query.bindValue(":notes", notes);
        query.bindValue(":cname", clientName);
        query.bindValue(":color", color.name());
        query.bindValue(":status", status);

        if (query.exec()) {
            id = query.lastInsertId().toInt();
            return true;
        } else {
            qDebug() << "Insert failed:" << query.lastError().text();
            return false;
        }
    }

    bool update(const QSqlDatabase &db) {
        if (id == 0) return false;

        QSqlQuery query(db);
        query.prepare("UPDATE appointments SET "
                      "clientId=:cid, userId=:uid, dateTime=:dt, durationMinutes=:dur, "
                      "serviceType=:stype, serviceCategory=:scat, priceType=:ptype, priceCharged=:pcharged, "
                      "notes=:notes, clientName=:cname, color=:color, status=:status WHERE id=:id");
        query.bindValue(":cid", clientId);
        query.bindValue(":uid", userId);
        query.bindValue(":dt", dateTime.toString(Qt::ISODate));
        query.bindValue(":dur", durationMinutes);
        query.bindValue(":stype", serviceType);
        query.bindValue(":scat", serviceCategory);   // ✅
        query.bindValue(":ptype", priceType);         // ✅
        query.bindValue(":pcharged", priceCharged);   // ✅
        query.bindValue(":notes", notes);
        query.bindValue(":cname", clientName);
        query.bindValue(":color", color.name());
        query.bindValue(":status", status);
        query.bindValue(":id", id);

        if (!query.exec()) {
            qDebug() << "Update failed:" << query.lastError().text();
            return false;
        }
        return true;
    }

    bool remove(const QSqlDatabase &db) {
        if (id == 0) return false;

        QSqlQuery query(db);
        query.prepare("DELETE FROM appointments WHERE id=:id");
        query.bindValue(":id", id);
        if (!query.exec()) {
            qDebug() << "Delete failed:" << query.lastError().text();
            return false;
        }
        return true;
    }
};

#endif // APPOINTMENT_H

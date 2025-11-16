#ifndef DOCUMENT_H
#define DOCUMENT_H

#include <QString>
#include <QDateTime>
#include <QFileInfo>
#include <optional>
#include <QSqlDatabase>
#include <QSqlQuery>
#include <QSqlError>
#include <QDebug>

class Document {
public:
    int id = 0;
    int userId = 0;    // The user who uploaded the document
    int clientId = 0;  // Optional: which client this document belongs to
    QString title;
    QString filePath;
    QDateTime createdAt = QDateTime::currentDateTime();

    Document() = default;

    QString displayName() const { return title.isEmpty() ? "[Untitled]" : title; }
    QString fileName() const { return QFileInfo(filePath).fileName(); }
    bool exists() const { return QFileInfo(filePath).exists(); }

    static std::optional<Document> getById(const QSqlDatabase &db, int docId) {
        QSqlQuery query(db);
        query.prepare("SELECT * FROM documents WHERE id=:id");
        query.bindValue(":id", docId);

        if (query.exec() && query.next()) {
            Document d;
            d.id = query.value("id").toInt();
            d.userId = query.value("userId").toInt();
            d.clientId = query.value("clientId").toInt();
            d.title = query.value("title").toString();
            d.filePath = query.value("filePath").toString();
            d.createdAt = QDateTime::fromString(query.value("createdAt").toString(), Qt::ISODate);
            return d;
        }
        return std::nullopt;
    }

    bool insert(const QSqlDatabase &db) {
        QSqlQuery query(db);
        query.prepare("INSERT INTO documents (userId, clientId, title, filePath, createdAt) "
                      "VALUES (:uid, :cid, :title, :path, :created)");
        query.bindValue(":uid", userId);
        query.bindValue(":cid", clientId);
        query.bindValue(":title", title);
        query.bindValue(":path", filePath);
        query.bindValue(":created", createdAt.toString(Qt::ISODate));

        if (query.exec()) {
            id = query.lastInsertId().toInt();
            return true;
        } else {
            qDebug() << "Document insert failed:" << query.lastError().text();
            return false;
        }
    }

    bool update(const QSqlDatabase &db) {
        if (id == 0) return false;

        QSqlQuery query(db);
        query.prepare("UPDATE documents SET userId=:uid, clientId=:cid, title=:title, filePath=:path, createdAt=:created WHERE id=:id");
        query.bindValue(":uid", userId);
        query.bindValue(":cid", clientId);
        query.bindValue(":title", title);
        query.bindValue(":path", filePath);
        query.bindValue(":created", createdAt.toString(Qt::ISODate));
        query.bindValue(":id", id);

        if (!query.exec()) {
            qDebug() << "Document update failed:" << query.lastError().text();
            return false;
        }
        return true;
    }

    bool remove(const QSqlDatabase &db) {
        if (id == 0) return false;

        QSqlQuery query(db);
        query.prepare("DELETE FROM documents WHERE id=:id");
        query.bindValue(":id", id);
        if (!query.exec()) {
            qDebug() << "Document delete failed:" << query.lastError().text();
            return false;
        }
        return true;
    }
};

#endif // DOCUMENT_H
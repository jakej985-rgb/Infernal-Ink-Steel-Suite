#include "xodosignclient.h"
#include <cpr/cpr.h>
#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonArray>
#include <QFile>
#include <filesystem>

XodoSignClient::XodoSignClient(const QString& apiKey, QObject *parent)
    : QObject(parent), m_apiKey(apiKey)
{
}

QJsonObject XodoSignClient::uploadDocument(const Document& document) {
    QFile file(document.filePath);
    if (!file.open(QIODevice::ReadOnly)) {
        return QJsonObject();
    }
    QByteArray fileData = file.readAll();

    // Corrected CPR multipart usage
    cpr::Multipart multipart{
        cpr::Part{"file", cpr::File{document.filePath.toStdString()}}
    };

    cpr::Session session;
    session.SetUrl(cpr::Url{"https://api.eversign.com/api/document"});
    session.SetParameters(cpr::Parameters{{"access_key", m_apiKey.toStdString()}});
    session.SetMultipart(multipart);

    cpr::Response r = session.Post();

    if (r.status_code == 200) {
        return QJsonDocument::fromJson(QByteArray::fromStdString(r.text)).object();
    }

    return QJsonObject();
}

QJsonObject XodoSignClient::createSignatureRequest(const QJsonObject& document, const QString& signerEmail) {
    QJsonObject payload;
    payload["sandbox"] = 1;
    payload["document_hash"] = document["document_hash"].toString();

    QJsonObject signer;
    signer["id"] = 1;
    signer["name"] = "Signer";
    signer["email"] = signerEmail;

    QJsonArray signers;
    signers.append(signer);
    payload["signers"] = signers;

    cpr::Session session;
    session.SetUrl(cpr::Url{"https://api.eversign.com/api/document"});
    session.SetParameters(cpr::Parameters{{"access_key", m_apiKey.toStdString()}});
    session.SetBody(cpr::Body{QJsonDocument(payload).toJson(QJsonDocument::Compact).toStdString()});
    session.SetHeader(cpr::Header{{"Content-Type", "application/json"}});

    cpr::Response r = session.Post();

    if (r.status_code == 200) {
        return QJsonDocument::fromJson(QByteArray::fromStdString(r.text)).object();
    }

    return QJsonObject();
}

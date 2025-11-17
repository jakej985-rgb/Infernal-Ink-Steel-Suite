#include "googlecalendarclient.h"
#include "browserdialog.h"

#include <QSettings>
#include <QDebug>
#include <QJsonDocument>
#include <QJsonObject>
#include <QUrlQuery>

#include "qjsonarray.h"
#include <cpr/cpr.h>

GoogleCalendarClient::GoogleCalendarClient(const QString& clientId,
                                           const QString& clientSecret,
                                           QObject *parent)
    : QObject(parent),
    m_clientId(clientId),
    m_clientSecret(clientSecret)
{
    loadTokens(); // Try to load previous tokens first
}

GoogleCalendarClient::~GoogleCalendarClient() = default;

void GoogleCalendarClient::setRedirectUri(const QString &uri)
{
    m_redirectUri = uri;
}

void GoogleCalendarClient::authenticate()
{
    // If we have a refresh token, try to use it first
    if (!m_refreshToken.isEmpty()) {
        qDebug() << "Using saved refresh token...";
        refreshAccessToken();
        return;
    }

    m_browserDialog = new BrowserDialog();
    connect(m_browserDialog, &BrowserDialog::urlChanged, [=](const QUrl& url) {
        if (url.toString().startsWith(m_redirectUri)) {
            QUrlQuery query(url);
            if (query.hasQueryItem("code")) {
                onAuthenticationSuccess(query.queryItemValue("code"));
                m_browserDialog->close();
            }
        }
    });

    QUrlQuery query;
    query.addQueryItem("client_id", m_clientId);
    query.addQueryItem("redirect_uri", m_redirectUri);
    query.addQueryItem("response_type", "code");
    query.addQueryItem("scope", "https://www.googleapis.com/auth/calendar");
    query.addQueryItem("access_type", "offline"); // request refresh token
    query.addQueryItem("prompt", "consent");

    QUrl auth_url("https://accounts.google.com/o/oauth2/v2/auth");
    auth_url.setQuery(query);

    m_browserDialog->load(auth_url);
    m_browserDialog->exec();
    delete m_browserDialog;
    m_browserDialog = nullptr;
}

void GoogleCalendarClient::onAuthenticationSuccess(const QString &code)
{
    QUrlQuery query;
    query.addQueryItem("client_id", m_clientId);
    query.addQueryItem("client_secret", m_clientSecret);
    query.addQueryItem("code", code);
    query.addQueryItem("grant_type", "authorization_code");
    query.addQueryItem("redirect_uri", m_redirectUri);

    std::string body = query.toString(QUrl::FullyEncoded).toStdString();

    qDebug() << "=== Google OAuth Token Request ===";
    qDebug() << "POST https://oauth2.googleapis.com/token";
    qDebug() << "Content-Type: application/x-www-form-urlencoded";
    qDebug() << "Body:" << QString::fromStdString(body);
    qDebug() << "===============================";

    cpr::Response r = cpr::Post(cpr::Url{"https://oauth2.googleapis.com/token"},
                                cpr::Body{body},
                                cpr::Header{{"Content-Type", "application/x-www-form-urlencoded"}});

    if (r.status_code != 200) {
        qWarning() << "Failed to obtain access token. Status:" << r.status_code;
        qWarning() << "Response body:" << r.text.c_str();
        emit authenticationFailed("Token exchange failed");
        return;
    }

    QJsonDocument doc = QJsonDocument::fromJson(r.text.c_str());
    QJsonObject obj = doc.object();

    m_accessToken = obj["access_token"].toString();
    m_refreshToken = obj["refresh_token"].toString();

    saveTokens();

    qDebug() << "Successfully obtained access token:" << m_accessToken.left(40) + "...";
    emit authenticationSucceeded();
}

void GoogleCalendarClient::refreshAccessToken()
{
    if (m_refreshToken.isEmpty()) {
        qWarning() << "No refresh token available.";
        authenticate();
        return;
    }

    QUrlQuery query;
    query.addQueryItem("client_id", m_clientId);
    query.addQueryItem("client_secret", m_clientSecret);
    query.addQueryItem("refresh_token", m_refreshToken);
    query.addQueryItem("grant_type", "refresh_token");

    cpr::Response r = cpr::Post(cpr::Url{"https://oauth2.googleapis.com/token"},
                                cpr::Body{query.toString(QUrl::FullyEncoded).toStdString()},
                                cpr::Header{{"Content-Type", "application/x-www-form-urlencoded"}});

    if (r.status_code != 200) {
        qWarning() << "Token refresh failed. Status:" << r.status_code;
        qWarning() << "Response:" << r.text.c_str();
        emit authenticationFailed("Refresh token invalid, need re-auth");
        return;
    }

    QJsonDocument doc = QJsonDocument::fromJson(r.text.c_str());
    QJsonObject obj = doc.object();
    m_accessToken = obj["access_token"].toString();

    saveTokens();
    qDebug() << "Access token refreshed successfully.";
    emit authenticationSucceeded();
}

void GoogleCalendarClient::loadTokens()
{
    QSettings settings("MyCompany", "ShopManager");
    m_accessToken = settings.value("google/access_token").toString();
    m_refreshToken = settings.value("google/refresh_token").toString();
}

void GoogleCalendarClient::saveTokens()
{
    QSettings settings("MyCompany", "ShopManager");
    settings.setValue("google/access_token", m_accessToken);
    settings.setValue("google/refresh_token", m_refreshToken);
}

void GoogleCalendarClient::listEvents()
{
    if (m_accessToken.isEmpty()) {
        qWarning() << "Access token missing. Attempting to refresh...";
        refreshAccessToken();
        if (m_accessToken.isEmpty()) {
            qWarning() << "Token refresh failed. Aborting event fetch.";
            return;
        }
    }

    cpr::Response r = cpr::Get(cpr::Url{"https://www.googleapis.com/calendar/v3/calendars/primary/events"},
                               cpr::Header{{"Authorization", "Bearer " + m_accessToken.toStdString()}});

    if (r.status_code == 200) {
        QJsonDocument json = QJsonDocument::fromJson(r.text.c_str());
        QJsonArray items = json.object()["items"].toArray();
        qDebug() << "=== Google Calendar Events ===";
        for (const auto& item : items) {
            qDebug() << item.toObject()["summary"].toString();
        }
    } else if (r.status_code == 401) {
        // Unauthorized → probably token expired
        qWarning() << "Access token expired. Refreshing...";
        refreshAccessToken();
    } else {
        qWarning() << "Failed to list events. Status:" << r.status_code;
        qWarning() << "Response:" << r.text.c_str();
    }
}

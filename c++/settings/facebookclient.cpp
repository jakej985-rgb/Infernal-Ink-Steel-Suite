#include "facebookclient.h"
#include "helper/browserdialog.h"
#include <cpr/cpr.h>
#include <QUrlQuery>
#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonArray>
#include <QFile>
#include <QFileInfo>
#include <QLoggingCategory>

#include "helper/facebookdebug.h"

Q_LOGGING_CATEGORY(facebookAuthLog, "social.facebook.auth")

namespace {

QString safeTokenPreview(const QString &token)
{
    if (token.isEmpty()) {
        return QStringLiteral("<empty>");
    }

    return token.left(6) + QStringLiteral("…");
}

} // namespace

FacebookClient::FacebookClient(const QString& appId, const QString& appSecret, QObject *parent)
    : QObject(parent), m_appId(appId), m_appSecret(appSecret)
{
    m_browserDialog = new BrowserDialog();
    connect(m_browserDialog, &BrowserDialog::urlChanged, this, &FacebookClient::onUrlChanged);
}

FacebookClient::~FacebookClient()
{
    delete m_browserDialog;
}

void FacebookClient::setRedirectUri(const QString& uri)
{
    const QString trimmedUri = uri.trimmed();
    QUrl parsed = QUrl::fromUserInput(trimmedUri);

    if (!parsed.isValid() || parsed.scheme().isEmpty()) {
        qCWarning(facebookAuthLog) << "⚠️ Invalid Facebook redirect URI provided:" << trimmedUri
                                   << "– keeping existing value";
        return;
    }

    const QString scheme = parsed.scheme().toLower();
    if (scheme != QStringLiteral("https") && scheme != QStringLiteral("http")) {
        qCWarning(facebookAuthLog) << "⚠️ Unusual redirect URI scheme" << scheme
                                   << "– ensure it matches the value configured in Facebook developer settings.";
    }

    if (scheme == QStringLiteral("http")) {
        const QString host = parsed.host().toLower();
        if (host != QStringLiteral("localhost") && host != QStringLiteral("127.0.0.1")) {
            qCWarning(facebookAuthLog) << "⚠️ Non-local HTTP redirect URI supplied:" << parsed.toString()
                                       << "– Facebook only allows insecure callbacks for localhost.";
        }
    }

    m_redirectUri = parsed.toString(QUrl::RemoveFragment);
    if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
        qCDebug(facebookAuthLog) << "Redirect URI set to:" << m_redirectUri;
    }
}

/**
 * Begin Facebook OAuth authentication
 */
void FacebookClient::authenticate() {
    if (m_redirectUri.isEmpty()) {
        m_redirectUri = "https://tattoo-shop-manager.vercel.app/auth/facebook/callback";
        qCWarning(facebookAuthLog) << "⚠️ No redirect URI set. Defaulting to" << m_redirectUri;
    }

    QUrlQuery query;
    query.addQueryItem("client_id", m_appId);
    query.addQueryItem("redirect_uri", m_redirectUri);
    query.addQueryItem("response_type", "code");

    // ✅ Updated: removed restricted scopes (pages_messaging)
    query.addQueryItem("scope",
                       "email,public_profile,"
                       "pages_show_list,pages_read_engagement,"
                       "pages_manage_posts,pages_manage_metadata");

    QUrl url("https://www.facebook.com/v19.0/dialog/oauth");
    url.setQuery(query);

    if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
        qCDebug(facebookAuthLog) << "OAuth URL constructed:" << url.toString();
        qCDebug(facebookAuthLog) << "Launching Facebook OAuth dialog";
    }

    m_browserDialog->load(url);

    const int dialogCode = m_browserDialog->exec();
    if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
        qCDebug(facebookAuthLog) << "Facebook OAuth dialog closed with code" << dialogCode;
    }
}

/**
 * Handle URL redirections during OAuth flow
 */
void FacebookClient::onUrlChanged(const QUrl& url) {
    const QString target = url.toString();
    if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
        qCDebug(facebookAuthLog) << "Browser redirected to:" << target;
    }

    if (target.startsWith(m_redirectUri)) {
        QUrlQuery query(url);
        if (query.hasQueryItem("code")) {
            QString code = query.queryItemValue("code");
            if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
                qCDebug(facebookAuthLog) << "✅ Received auth code:" << safeTokenPreview(code);
            }
            m_browserDialog->accept();
            exchangeCodeForToken(code);
        } else if (query.hasQueryItem("error")) {
            QString err = query.queryItemValue("error_description");
            if (err.isEmpty()) {
                err = query.queryItemValue("error_reason");
            }
            if (err.isEmpty()) {
                err = query.queryItemValue("error");
            }
            if (err.isEmpty()) {
                err = QStringLiteral("Unknown OAuth error");
            }
            qCWarning(facebookAuthLog) << "❌ Facebook returned error:" << err
                                       << "at URL" << target;
            m_browserDialog->reject();
            if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
                qCDebug(facebookAuthLog) << "Emitting authenticationFailed signal after OAuth error";
            }
            emit authenticationFailed(err);
        }
    }
}

/**
 * Exchange OAuth code for long-lived access token
 * (https://developers.facebook.com/docs/facebook-login/guides/access-tokens)
 */
void FacebookClient::exchangeCodeForToken(const QString& code) {
    if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
        qCDebug(facebookAuthLog) << "Exchanging code for token with redirect_uri:" << m_redirectUri;
    }

    cpr::Response r = cpr::Get(cpr::Url{"https://graph.facebook.com/v19.0/oauth/access_token"},
                               cpr::Parameters{
                                   {"client_id", m_appId.toStdString()},
                                   {"redirect_uri", m_redirectUri.toStdString()},
                                   {"client_secret", m_appSecret.toStdString()},
                                   {"code", code.toStdString()}
                               });

    if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
        qCDebug(facebookAuthLog) << "Token exchange HTTP status:" << r.status_code
                                 << "error:" << QString::fromStdString(r.error.message);
        qCDebug(facebookAuthLog) << "Token exchange response payload:" << QString::fromStdString(r.text);
    }

    if (r.status_code == 200) {
        QJsonDocument jsonDoc = QJsonDocument::fromJson(r.text.c_str());
        QJsonObject jsonObj = jsonDoc.object();
        m_accessToken = jsonObj["access_token"].toString();
        if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
            qCDebug(facebookAuthLog) << "✅ Access token received:" << safeTokenPreview(m_accessToken);
            qCDebug(facebookAuthLog) << "Emitting authenticated signal";
        }
        emit authenticated(m_accessToken);
    } else {
        qCWarning(facebookAuthLog) << "❌ Token exchange failed with status" << r.status_code
                                   << "error:" << QString::fromStdString(r.error.message)
                                   << "payload:" << QString::fromStdString(r.text);
        if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
            qCDebug(facebookAuthLog) << "Emitting authenticationFailed signal due to token exchange failure";
        }
        const QString responseError = QString::fromStdString(r.error.message);
        const QString responseText = QString::fromStdString(r.text);
        const QString detail = responseError.trimmed().isEmpty() ? responseText : responseError;
        emit authenticationFailed(QStringLiteral("Token exchange failed (HTTP %1): %2")
                                       .arg(r.status_code)
                                       .arg(detail));
    }
}

/**
 * Fetch user profile
 * (https://developers.facebook.com/docs/graph-api/reference/user/)
 */
void FacebookClient::fetchUserProfile() {
    if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
        qCDebug(facebookAuthLog) << "Fetching Facebook profile with token:" << safeTokenPreview(m_accessToken);
    }

    cpr::Response r = cpr::Get(cpr::Url{"https://graph.facebook.com/v19.0/me"},
                               cpr::Parameters{
                                   {"fields", "id,name,picture{url}"},
                                   {"access_token", m_accessToken.toStdString()}
                               });

    if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
        qCDebug(facebookAuthLog) << "Profile request status" << r.status_code
                                 << "error:" << QString::fromStdString(r.error.message);
        qCDebug(facebookAuthLog) << "Profile response payload:" << QString::fromStdString(r.text);
    }

    if (r.status_code == 200) {
        QJsonDocument doc = QJsonDocument::fromJson(r.text.c_str());
        QJsonObject obj = doc.object();

        FacebookProfile profile;
        profile.id = obj["id"].toString();
        profile.name = obj["name"].toString();
        if (obj.contains("picture"))
            profile.pictureUrl = QUrl(obj["picture"].toObject()["data"].toObject()["url"].toString());

        if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
            qCDebug(facebookAuthLog) << "✅ User profile fetched:" << profile.name << "(" << profile.id << ")";
            qCDebug(facebookAuthLog) << "Emitting userProfileFetched signal";
        }
        emit userProfileFetched(profile);
    } else {
        qCWarning(facebookAuthLog) << "❌ Failed to fetch user profile (status" << r.status_code
                                   << ") error:" << QString::fromStdString(r.error.message)
                                   << "payload:" << QString::fromStdString(r.text);
        if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
            qCDebug(facebookAuthLog) << "Emitting authenticationFailed signal due to profile failure";
        }
        emit authenticationFailed(QStringLiteral("Failed to fetch user profile (HTTP %1)").arg(r.status_code));
    }
}

/**
 * Get Pages user manages
 * (https://developers.facebook.com/docs/graph-api/reference/user/accounts/)
 */
void FacebookClient::fetchPages() {
    if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
        qCDebug(facebookAuthLog) << "Fetching Pages for user with token:" << safeTokenPreview(m_accessToken);
    }

    cpr::Response r = cpr::Get(cpr::Url{"https://graph.facebook.com/v19.0/me/accounts"},
                               cpr::Parameters{
                                   {"fields", "id,name,access_token"},
                                   {"access_token", m_accessToken.toStdString()}
                               });

    if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
        qCDebug(facebookAuthLog) << "Pages request status" << r.status_code
                                 << "error:" << QString::fromStdString(r.error.message);
        qCDebug(facebookAuthLog) << "Pages response payload:" << QString::fromStdString(r.text);
    }

    if (r.status_code == 200) {
        QJsonDocument doc = QJsonDocument::fromJson(r.text.c_str());
        QJsonArray data = doc.object()["data"].toArray();

        QList<FacebookPageData> pages;
        for (QJsonValue p : data) {
            QJsonObject o = p.toObject();
            FacebookPageData pg;
            pg.id = o["id"].toString();
            pg.name = o["name"].toString();
            pg.accessToken = o["access_token"].toString();
            pages.append(pg);
        }

        if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
            qCDebug(facebookAuthLog) << "✅ Fetched" << pages.size() << "pages.";
            qCDebug(facebookAuthLog) << "Emitting pagesFetched signal";
        }
        emit pagesFetched(pages);
    } else {
        qCWarning(facebookAuthLog) << "❌ Failed to fetch Pages (status" << r.status_code
                                   << ") error:" << QString::fromStdString(r.error.message)
                                   << "payload:" << QString::fromStdString(r.text);
        if (isFacebookDebugLoggingEnabled(facebookAuthLog())) {
            qCDebug(facebookAuthLog) << "Emitting authenticationFailed signal due to pages failure";
        }
        emit authenticationFailed(QStringLiteral("Failed to fetch Pages list (HTTP %1)").arg(r.status_code));
    }
}

/**
 * Get Page conversations (requires pages_manage_metadata)
 * (https://developers.facebook.com/docs/graph-api/reference/page/conversations/)
 */
void FacebookClient::fetchConversations(const QString& pageId, const QString& pageAccessToken) {
    qDebug() << "Fetching conversations for page:" << pageId;

    cpr::Response r = cpr::Get(cpr::Url{"https://graph.facebook.com/v19.0/" + pageId.toStdString() + "/conversations"},
                               cpr::Parameters{
                                   {"fields", "id,snippet,updated_time"},
                                   {"access_token", pageAccessToken.toStdString()}
                               });

    if (r.status_code == 200) {
        QJsonDocument doc = QJsonDocument::fromJson(r.text.c_str());
        QJsonArray arr = doc.object()["data"].toArray();

        QList<FacebookConversation> convs;
        for (QJsonValue v : arr) {
            QJsonObject o = v.toObject();
            FacebookConversation c;
            c.id = o["id"].toString();
            c.snippet = o["snippet"].toString();
            c.updated_time = o["updated_time"].toString();
            convs.append(c);
        }

        qDebug() << "✅ Fetched" << convs.size() << "conversations.";
        emit conversationsFetched(convs);
    } else {
        qWarning() << "❌ Failed to fetch conversations:" << QString::fromStdString(r.text);
        emit authenticationFailed("Failed to fetch conversations.");
    }
}

/**
 * Get messages from a conversation
 * (https://developers.facebook.com/docs/graph-api/reference/conversation/messages/)
 */
void FacebookClient::fetchMessages(const QString& conversationId, const QString& pageAccessToken) {
    qDebug() << "Fetching messages for conversation:" << conversationId;

    cpr::Response r = cpr::Get(cpr::Url{"https://graph.facebook.com/v19.0/" + conversationId.toStdString() + "/messages"},
                               cpr::Parameters{
                                   {"fields", "id,message,from,created_time"},
                                   {"access_token", pageAccessToken.toStdString()}
                               });

    if (r.status_code == 200) {
        QJsonDocument doc = QJsonDocument::fromJson(r.text.c_str());
        QJsonArray data = doc.object()["data"].toArray();

        QList<FacebookMessage> msgs;
        for (QJsonValue v : data) {
            QJsonObject o = v.toObject();
            FacebookMessage m;
            m.id = o["id"].toString();
            m.message = o["message"].toString();
            m.from = o["from"].toObject()["name"].toString();
            m.created_time = o["created_time"].toString();
            msgs.append(m);
        }

        qDebug() << "✅ Retrieved" << msgs.size() << "messages.";
        emit messagesFetched(msgs);
    } else {
        qWarning() << "❌ Failed to fetch messages:" << QString::fromStdString(r.text);
        emit authenticationFailed("Failed to fetch messages.");
    }
}

void FacebookClient::sendMessage(const QString& conversationId, const QString& message, const QString& pageAccessToken) {
    qDebug() << "Sending message to conversation:" << conversationId;

    if (conversationId.isEmpty()) {
        qWarning() << "❌ Cannot send Facebook message: conversationId is empty.";
        emit messageSent(false);
        return;
    }

    if (message.trimmed().isEmpty()) {
        qWarning() << "❌ Cannot send Facebook message: message body is empty.";
        emit messageSent(false);
        return;
    }

    std::string endpoint = "https://graph.facebook.com/v19.0/" + conversationId.toStdString() + "/messages";
    cpr::Response response = cpr::Post(
        cpr::Url{endpoint},
        cpr::Parameters{
            {"message", message.toStdString()},
            {"access_token", pageAccessToken.toStdString()}
        }
    );

    if (response.status_code == 200) {
        qDebug() << "✅ Facebook message sent successfully.";
        emit messageSent(true);
    } else {
        qWarning() << "❌ Failed to send Facebook message:" << QString::fromStdString(response.text)
                   << "(status" << response.status_code << ")";
        emit messageSent(false);
    }
}

/**
 * Create a new photo or text post
 * (https://developers.facebook.com/docs/graph-api/reference/page/photos/)
 */
void FacebookClient::createPost(const QString& pageId, const QString& message, const QString& imagePath, const QString& pageAccessToken) {
    qDebug() << "Creating post on page:" << pageId << "with image:" << imagePath;

    std::vector<cpr::Part> parts = {
        {"message", message.toStdString()},
        {"access_token", pageAccessToken.toStdString()}
    };

    QString endpoint;
    if (imagePath.isEmpty()) {
        endpoint = QString("https://graph.facebook.com/v19.0/%1/feed").arg(pageId);
    } else {
        endpoint = QString("https://graph.facebook.com/v19.0/%1/photos").arg(pageId);
        parts.emplace_back("source", cpr::File{imagePath.toStdString()});
    }

    cpr::Response r = cpr::Post(cpr::Url{endpoint.toStdString()}, cpr::Multipart(parts));

    qDebug() << "Post creation response:" << r.status_code << QString::fromStdString(r.text);
    emit postCreated(r.status_code == 200);
}

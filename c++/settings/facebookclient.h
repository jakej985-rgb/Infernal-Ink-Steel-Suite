#pragma once

#include <QObject>
#include <QString>
#include <QUrl>
#include <QList>
#include <QLoggingCategory>

class BrowserDialog;

struct FacebookMessage {
    QString id;
    QString message;
    QString from;
    QString created_time;
};

struct FacebookConversation {
    QString id;
    QString snippet;
    QString updated_time;
    QList<FacebookMessage> messages;
};

struct FacebookPageData {
    QString id;
    QString name;
    QString accessToken;
};

struct FacebookProfile {
    QString id;
    QString name;
    QUrl pictureUrl;
};

class FacebookClient : public QObject {
    Q_OBJECT

public:
    explicit FacebookClient(const QString& appId, const QString& appSecret, QObject *parent = nullptr);
    ~FacebookClient();

    void setRedirectUri(const QString& uri);
    void authenticate();
    void fetchUserProfile();
    void fetchPages();
    void fetchConversations(const QString& pageId, const QString& pageAccessToken);
    void fetchMessages(const QString& conversationId, const QString& pageAccessToken);
    void sendMessage(const QString& conversationId, const QString& message, const QString& pageAccessToken);
    void createPost(const QString& pageId, const QString& message, const QString& imagePath, const QString& pageAccessToken);

signals:
    void authenticated(const QString& accessToken);
    void authenticationFailed(const QString& error);
    void userProfileFetched(const FacebookProfile& profile);
    void pagesFetched(const QList<FacebookPageData>& pages);
    void conversationsFetched(const QList<FacebookConversation>& conversations);
    void messagesFetched(const QList<FacebookMessage>& messages);
    void messageSent(bool success);
    void postCreated(bool success);

private slots:
    void onUrlChanged(const QUrl& url);

private:
    void exchangeCodeForToken(const QString& code);

    QString m_appId;
    QString m_appSecret;
    QString m_redirectUri;
    QString m_accessToken;

    BrowserDialog* m_browserDialog;
};

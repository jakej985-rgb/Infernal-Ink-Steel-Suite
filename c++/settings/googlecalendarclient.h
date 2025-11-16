#ifndef GOOGLECALENDARCLIENT_H
#define GOOGLECALENDARCLIENT_H

#include <QObject>
#include <QString>

class BrowserDialog;

class GoogleCalendarClient : public QObject
{
    Q_OBJECT

public:
    explicit GoogleCalendarClient(const QString& clientId,
                                  const QString& clientSecret,
                                  QObject *parent = nullptr);
    ~GoogleCalendarClient();

    void setRedirectUri(const QString& uri);
    void authenticate();
    void listEvents();
    void refreshAccessToken();

signals:
    void authenticationSucceeded();
    void authenticationFailed(const QString &error);

private slots:
    void onAuthenticationSuccess(const QString &code);

private:
    void loadTokens();   // load from QSettings
    void saveTokens();   // save to QSettings

    QString m_clientId;
    QString m_clientSecret;
    QString m_redirectUri;

    QString m_accessToken;
    QString m_refreshToken;

    BrowserDialog *m_browserDialog;
};

#endif // GOOGLECALENDARCLIENT_H

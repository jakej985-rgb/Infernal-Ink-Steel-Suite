#pragma once

#include "themes/themeablewidget.h"
#include "helper/googlecalendarclient.h"
#include "helper/facebookclient.h"
#include <QPushButton>
#include <QLabel>
#include <QComboBox>
#include <QVector>

class GlowAvatar;
class QVBoxLayout;
class SettingsCard;
class QNetworkAccessManager;
class QNetworkReply;

class LinkedAccountsTab : public ThemeableWidget {
    Q_OBJECT

public:
    explicit LinkedAccountsTab(FacebookClient* client, QWidget *parent = nullptr);

signals:
    void facebookAccountLinked(bool linked);
    void facebookPageSelected(const FacebookPageData& page);

private slots:
    void linkGoogleAccount();
    void linkFacebookAccount();
    void onFacebookAuthenticationFinished(const QString& accessToken);
    void onFacebookAuthenticationFailed(const QString& error);
    void onFacebookProfileFetched(const FacebookProfile& profile);
    void onFacebookPagesFetched(const QList<FacebookPageData>& pages);
    void onAvatarDownloaded(QNetworkReply* reply);

private:
    void setupFacebookUI(QVBoxLayout *contentLayout);
    void updateTheme() override;
    QWidget *createSectionHeader(const QString &iconPath, const QString &title);
    QLabel *createCaptionLabel(const QString &text);

    GoogleCalendarClient* m_googleCalendarClient;
    SettingsCard* m_googleCard = nullptr;
    QPushButton* m_linkGoogleAccountButton;
    QLabel* m_googleAccountStatusLabel;

    FacebookClient* m_facebookClient;
    SettingsCard* m_facebookCard = nullptr;
    QPushButton* m_linkFacebookAccountButton;
    QWidget* m_facebookProfileCard;
    GlowAvatar* m_facebookAvatar;
    QLabel* m_facebookNameLabel;
    QComboBox* m_facebookPagesComboBox;
    QList<FacebookPageData> m_facebookPages;
    QVector<QLabel*> m_headingLabels;
    QVector<QLabel*> m_captionLabels;
    QNetworkAccessManager* m_avatarNetworkManager = nullptr;
};

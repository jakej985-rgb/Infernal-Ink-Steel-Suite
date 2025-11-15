#pragma once

#include "themes/themeablewidget.h"
#include <QListWidget>
#include <QTextEdit>
#include <QPushButton>
#include "helper/facebookclient.h"

class InboxTab : public ThemeableWidget {
    Q_OBJECT

public:
    explicit InboxTab(FacebookClient* client, QWidget *parent = nullptr);

    void setCurrentPage(const FacebookPageData& page);

private slots:
    void onConversationsFetched(const QList<FacebookConversation>& conversations);
    void onMessagesFetched(const QList<FacebookMessage>& messages);
    void onMessageSent(bool success);
    void onConversationSelected(QListWidgetItem* item);
    void onSendClicked();

private:
    FacebookClient* m_facebookClient;
    FacebookPageData m_currentPage;
    QListWidget* m_conversationsList;
    QListWidget* m_messagesList;
    QTextEdit* m_replyTextEdit;
    QPushButton* m_sendButton;
};

#pragma once
#include "themes/themeablewidget.h"
#include "db/userdb.h"

#include <QVector>

class QLineEdit;
class QPushButton;
class QLabel;
class SettingsCard;

class UserTab : public ThemeableWidget
{
    Q_OBJECT
public:
    explicit UserTab(UserDB &udb, const QString &username, QWidget *parent = nullptr);

private:
    void updateTheme() override;

    UserDB &userDB;
    QString username;
    SettingsCard *userCard = nullptr;
    QLineEdit *newPass;
    QPushButton *changeBtn;
    QVector<QLabel*> headingLabels;
    QVector<QLabel*> captionLabels;

    QWidget *createSectionHeader(const QString &iconPath, const QString &title);
    QLabel *createCaptionLabel(const QString &text);
};

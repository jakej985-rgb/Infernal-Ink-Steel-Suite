#pragma once
#include "themes/themeablewidget.h"
#include "db/userdb.h"

#include <QVector>

class QLineEdit;
class QComboBox;
class QPushButton;
class QLabel;
class SettingsCard;

class ManagerTab : public ThemeableWidget
{
    Q_OBJECT
public:
    explicit ManagerTab(UserDB &udb, QWidget *parent = nullptr);

private:
    void updateTheme() override;
    QStringList allRoles();
    QStringList splitRoles(const QString &roleStr);

    UserDB &userDB;
    SettingsCard *managerCard = nullptr;
    QLineEdit *staffEdit;
    QComboBox *rolesCombo;
    QPushButton *updateBtn;
    QVector<QLabel*> headingLabels;
    QVector<QLabel*> captionLabels;
    QVector<QLabel*> bodyLabels;

    QWidget *createSectionHeader(const QString &iconPath, const QString &title);
    QLabel *createCaptionLabel(const QString &text);
};

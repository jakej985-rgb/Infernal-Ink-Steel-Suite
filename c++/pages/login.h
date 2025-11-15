#ifndef LOGIN_H
#define LOGIN_H

#include <QWidget>
#include <QLineEdit>
#include <QLabel>
#include <QCheckBox>
#include <QGridLayout>
#include <QPushButton>
#include <QVBoxLayout>
#include <QScopedPointer>
#include <QScrollArea>

#include "helper/glowavatar.h"
#include "helper/neonbutton.h"
#include "db/userdb.h"
#include "db/shopsettingsdb.h"
#include "db/databasemanager.h"
#include "dashboard.h"
#include "themes/themeablewidget.h"
#include <QColor>

class Login : public ThemeableWidget {
    Q_OBJECT
public:
    explicit Login(UserDB* udb, ShopSettingDB* sdb, QWidget *parent = nullptr);

protected:
    void resizeEvent(QResizeEvent *event) override;

private slots:
    void onSignInClicked();
    void toggleShowPassword();
    void onBackClicked();
    void showAfterLogout();

private:
    void updateTheme() override;
    void applyBranding();
    // --- DB ---
    UserDB* userDB;
    ShopSettingDB* settingsDb;
    QScopedPointer<ClientDB> m_clientDb;
    QScopedPointer<AppointmentDB> m_appointmentDb;
    QScopedPointer<DocumentStorageDB> m_documentsDb;

    // --- Widgets ---
    QWidget *m_gridContainer;
    QScrollArea *m_userScrollArea;
    QGridLayout *m_grid;
    QWidget *m_userLoginView;
    QWidget *m_userHeaderRow;
    QWidget *m_brandingPanel;
    QLabel *m_headlineLabel;
    QLabel *m_taglineLabel;
    GlowAvatar *m_avatarLabel;
    QLabel *m_selectedUserLabel;
    QLineEdit *m_passwordEdit;
    QCheckBox *m_showPasswordCheck;
    NeonButton *m_signInBtn;
    NeonButton *m_backBtn;

    User m_currentUser;
    QColor m_brandAccent;
    QString m_backgroundPath;

    // --- Helpers ---
    QPixmap makeLetterPixmap(const QString &initial, int size, const QColor &base = QColor(60,120,180)) const;
    void buildUserGrid();
    void showUserSelected(const User &user);
};

#endif // LOGIN_H
